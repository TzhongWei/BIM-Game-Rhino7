using System;
using System.Collections.Generic;
using System.Linq;

using Rhino;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace CirBIMGame.grasshopper
{
    public class Target : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Target class.
        /// </summary>
        public Target()
          : base("Target", "T",
              "This component re-orients the camera in rhino targeting to the input geometry",
              "CirBIMGame", "Util")
        {
        }
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            int[] stringID = new int[] { 0 };

            Param_Boolean in0Str = Params.Input[stringID[0]] as Param_Boolean;
            if (in0Str == null || in0Str.SourceCount > 0 || in0Str.PersistentDataCount > 0) return;
            int x = (int)in0Str.Attributes.Pivot.X - 250;
            int y = (int)in0Str.Attributes.Pivot.Y - 10;
            GH_ButtonObject gH_ButtonObject = new GH_ButtonObject();
            gH_ButtonObject.Attributes.Pivot = new System.Drawing.PointF(x, y);
            document.AddObject(gH_ButtonObject, false);
            in0Str.AddSource(gH_ButtonObject);
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Find", "F", "Find the input geometries", GH_ParamAccess.item);
            pManager.AddGeometryParameter("GeometryBases", "G", "The geometries need to be targeted", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        public Box BoundingBoxOfGeoms(IEnumerable<GeometryBase> Geoms)
        {
            var BBox = Geoms.Select(x => new Box(x.GetBoundingBox(true))).ToList();
            var X = BBox[0].X;
            var Y = BBox[0].Y;
            var Z = BBox[0].Z;

            foreach (var item in BBox)
            {
                if (item.X.Min < X.Min)
                    X = new Interval(item.X.Min, X.Max);

                if (item.X.Max > X.Max)
                    X = new Interval(X.Min, item.X.Max);

                if (item.Y.Min < Y.Min)
                    Y = new Interval(item.Y.Min, Y.Max);

                if (item.Y.Max > Y.Max)
                    Y = new Interval(Y.Min, item.Y.Max);

                if (item.Z.Min < Z.Min)
                    Z = new Interval(item.Z.Min, Z.Max);

                if (item.X.Max > X.Max)
                    Z = new Interval(Z.Min, item.Z.Max);
            }
            return new Box(Plane.WorldXY, X, Y, Z);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Find = false;
            var Geoms = new List<GeometryBase>();
            DA.GetDataList("GeometryBases", Geoms);
            DA.GetData("Find", ref Find);
            if (Geoms == null) return;
            // Write your logic here
            var Boundary = BoundingBoxOfGeoms(Geoms);
            Boundary.Transform(Transform.Scale(new Plane(Boundary.Center, Vector3d.ZAxis), 1, 1, 1.2));
            var CamLocation = new Point3d(Boundary.X.Max, Boundary.Y.Max, Boundary.Z.Min);
            var CamTarget = new Point3d(Boundary.X.Min, Boundary.Y.Min, Boundary.Z.Max);
            var Doc = RhinoDoc.ActiveDoc;
            if (Find)
            {
                Doc.Views.ActiveView.ActiveViewport.SetCameraLocations(CamLocation, CamTarget);

            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("98ECE0D2-E510-42B9-8661-B417C363D98A"); }
        }
    }
}