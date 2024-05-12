using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using CirBIMGame;

namespace CirBIMGame.grasshopper
{
    public class FacadePatternSetting : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public FacadePatternSetting()
          : base("FacadePatternSetting", "FcDPat",
            "This is the setting for the facade based on the face index and percentages",
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
            pManager.AddNumberParameter("VoidRate", "V", "The opening ratio setting for the building facade", GH_ParamAccess.item);
            pManager.AddNumberParameter("BlueRate", "B", "The blue brick ratio setting for the building facade", GH_ParamAccess.item);
            pManager.AddNumberParameter("YellowRate", "Y", "The yellow brick ratio setting for the building facade", GH_ParamAccess.item);
            pManager.AddNumberParameter("NormalRate", "N", "The white brick ratio setting for the building facade", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("FacadeSetting", "FcS", 
                "The facade data setting represented with json format.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Result = new Dictionary<string, double>();

            double VoidRate = 0, BlueBrick = 0, YellowBrick = 0, NormalBrick = 0;
            int Index = 0;
            DA.GetData("Index", ref Index);
            DA.GetData("VoidRate", ref VoidRate);
            DA.GetData("BlueRate", ref BlueBrick);
            DA.GetData("YellowRate", ref YellowBrick);
            DA.GetData("NormalRate", ref NormalBrick);

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


            var Data = CirBIMGame.JsonFormat.Set(Index, Result);
            DA.SetData("FacadeData", Data.ToJson());
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("B9516CEC-3ED9-491F-86F3-22F7ABC03FE7");
    }
}