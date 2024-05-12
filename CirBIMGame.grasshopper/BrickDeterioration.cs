using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CirBIMGame.grasshopper
{
    public class BrickDeterioration : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BrickDeterioration class.
        /// </summary>
        public BrickDeterioration()
          : base("BrickDeterioration", "BrickDer",
              "This component employ the Markov Chain to predict the quality of bricks after a period",
              "CirBIMGame", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Brick", "B", "The total unit number of bricks", GH_ParamAccess.item);
            pManager.AddTextParameter("Matrix", "M", "The possibility matrix of deterioration rate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Period", "P", "The building ages in the simulation", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("DeteriorationVector", "DetVec", "The Deterioration vector from the Markov chain", GH_ParamAccess.item);
            pManager.AddTextParameter("BrickQuality", "BDet", "The list of brick quality after a period", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Duration", "D", "The duration of the brick type based on the possibilty matrix", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int brick = 1;
            string Matrix = "";
            int Period = 1;
            DA.GetData("Brick", ref brick);
            DA.GetData("Matrix", ref Matrix);
            DA.GetData("Period", ref Period);
            if (Matrix == "" || Matrix == null) return;
            var P = new matrix(1, 6);
            P[0, 0] = 1;
            //Write your logic here
            var Mt = new matrix(Matrix);
            var Result = P * matrix.Pow(Mt, Period);
            DA.SetData("DeteriorationVector", Result.ToString());
            
            var JsonObj = new Dictionary<string, double>{
        {"Good", brick * Result[0, 0]},
        {"MidGood", brick * Result[0, 1]},
        {"NorGood", brick * Result[0, 2]},
        {"NorBad", brick * Result[0, 3]},
        {"MidBad", brick * Result[0, 4]},
        {"Bad", brick * Result[0, 5]}
        };
            DA.SetData("BrickQuality", Util.Serialise(JsonObj));
            DA.SetData("Duration", matrix.DurationEva(P, Mt));
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
            get { return new Guid("1B8F6A53-A117-4F91-80D6-4E513F2AFB25"); }
        }
    }
}