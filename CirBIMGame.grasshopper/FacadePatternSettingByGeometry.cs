using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CirBIMGame.grasshopper
{
    public class FacadePatternSettingByGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public FacadePatternSettingByGeometry()
          : base("FacadePatternSettingByGeometry", "FcDPatGeom",
              "This is the setting for the facade based on the face index and brep area from the geometry setting",
              "CirBIMGame", "Setting")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "I",
                "Please provide index to assign the settings for your building.", GH_ParamAccess.item, 0);
            pManager.AddBrepParameter("Face", "F", "The face from the mass", GH_ParamAccess.item);
            pManager.AddBrepParameter("VoidArea", "V", "The opening area setting for the building facade", GH_ParamAccess.list);
            pManager.AddBrepParameter("BlueArea", "B", "The blue brick area setting for the building facade", GH_ParamAccess.list);
            pManager.AddBrepParameter("YellowArea", "Y", "The yellow brick area setting for the building facade", GH_ParamAccess.list);
            pManager.AddBrepParameter("NormalArea", "N", "The white brick area setting for the building facade", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("FacadeSetting", "FcS",
    "The facade data setting represented with json format.", GH_ParamAccess.item);
        }
        private double SUM(IEnumerable<Brep> Region, Brep Face)
        {
            if (Region == null)
                return 0;
            double Area = Face.GetArea();
            double total = 0;
            foreach (var item in Region)
            {
                total += item.GetArea();
            }
            return total / Area * 100;
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> VoidArea = new List<Brep>()
                , BlueArea = new List<Brep>()
                , YellowArea = new List<Brep>()
                , NormalArea = new List<Brep>();
            int Index = 0;
            Brep Face = new Brep();
            DA.GetData("Index", ref Index);
            DA.GetData("Face", ref Face);
            DA.GetDataList("VoidArea", VoidArea);
            DA.GetDataList("BlueArea", BlueArea);
            DA.GetDataList("YellowArea", YellowArea);
            DA.GetDataList("NormalArea", NormalArea);

            var VoidRate = SUM(VoidArea, Face);
            var BlueBrick = SUM(BlueArea, Face);
            var YellowBrick = SUM(YellowArea, Face);
            var NormalBrick = SUM(NormalArea, Face);

            var Result = new Dictionary<string, double>();
            if (VoidRate + BlueBrick + YellowBrick + NormalBrick == 100)
            {
                Result = new Dictionary<string, double>(){
          {"voidrate", VoidRate},
          {"bluebrick", BlueBrick},
          {"yellowbrick", YellowBrick},
          {"normalbrick", NormalBrick}
          };
            }
            else if (VoidRate + BlueBrick + YellowBrick + NormalBrick > 100)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The Sum of the rates cannot larger than 100");

            }
            else
            {
                var Rates = Util.RateCalculator(new double[4] { VoidRate, BlueBrick, YellowBrick, NormalBrick });

                Result = new Dictionary<string, double>(){
          {"voidrate", Math.Round(Rates[0], 6)},
          {"bluebrick", Math.Round(Rates[1], 6)},
          {"yellowbrick", Math.Round(Rates[2], 6)},
          {"normalbrick", Math.Round(Rates[3], 6)}
          };
            }

            var Data = JsonFormat.Set(Index, Result);
            DA.SetData("FacadeData", Data.ToJson());
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
            get { return new Guid("C6987DF6-99C3-44CB-A827-B45B107ED9CA"); }
        }
    }
}