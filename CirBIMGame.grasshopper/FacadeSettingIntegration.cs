using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CirBIMGame.grasshopper
{
    public class FacadeSettingIntegration : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public FacadeSettingIntegration()
          : base("FacadeSettingIntegration", "FaDMerge",
              "This component is used to analyse the facade information from the settings ",
               "CirBIMGame", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Mass", "M", "The building mass", GH_ParamAccess.list);
            pManager.AddTextParameter("DataSetting", "FcS", "The facade settings for the mass", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Building", "B", "The building mass", GH_ParamAccess.item);
            pManager.AddBrepParameter("Facades", "Fs", "The facade of the building", GH_ParamAccess.list);
            pManager.AddTextParameter("FacadeSetting", "FcS", "The Facade data of the building mass", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Mass = new List<Brep>();
            var DataSetting = new List<string>();
            DA.GetDataList("Mass", Mass);
            DA.GetDataList("DataSetting", DataSetting);
            var union = Union(Mass);
            var WallList = Walls(union);
            var FacadeData_1 = new FacadeSetting(WallList);
            FacadeData_1.Setting(DataSetting);
            DA.SetDataList("FacadeSetting", FacadeData_1.Print());
            DA.SetDataList("Facades", WallList);
            DA.SetData("Building", union);
        }
        public List<Brep> Walls(Brep Mass)
        {
            var BrepFaces = Mass.Faces;
            var WallList = new List<Brep>();
            foreach (var face in BrepFaces)
            {
                if (!face.IsPlanar())
                    WallList.Add(face.DuplicateFace(false));

                var DomU = face.Domain(0);
                var DomV = face.Domain(1);

                var Normal = face.NormalAt(DomU.Mid, DomV.Mid);
                if (Math.Abs(Normal * Vector3d.ZAxis) == 1
                  || !face.IsPlanar()) continue;
                WallList.Add(face.DuplicateFace(false));
            }
            return WallList;
        }
        public Brep Union(IEnumerable<Brep> Massive)
        {
            var Union = Brep.CreateBooleanUnion(Massive, 0.1);
            if (Union.Length != 1)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Union Failed");
                return null;
            }
            Union[0].MergeCoplanarFaces(0.1);
            return Union[0];
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
            get { return new Guid("F4765F74-B371-4EFD-BF41-A21D630BD920"); }
        }
    }
}