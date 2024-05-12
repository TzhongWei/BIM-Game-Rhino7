using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CirBIMGame.grasshopper
{
    public class FacadePrice : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FacadePrice class.
        /// </summary>
        public FacadePrice()
          : base("FacadePrice", "FPrice",
              "This component analyses the facade prices based on the setting of the brick amount",
              "CirBIMGame", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FacadeData", "FcD", "The facade data from the Facade setting integration component", GH_ParamAccess.list);
            pManager.AddNumberParameter("UnitScale", "UnitS", "The percentages of each unit number", GH_ParamAccess.list);
            pManager.AddNumberParameter("AveragePrice", "Price", "The average price of the brick", GH_ParamAccess.item);
            pManager.AddNumberParameter("PriceWeight", "Weight", "The weight of the unit prices", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "D", "The data generated from this function depicts the information of the brick types", GH_ParamAccess.item);
            pManager.AddNumberParameter("FacadeTotalPrice", "T", "The total construction price for the facade", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var FcD = new List<string>(); 
            var UnitScale = new List<double>(); 
            var PriceWeight = new List<double>();
            var AveragePrice = 1.0;
            DA.GetDataList("FacadeData", FcD);
            DA.GetDataList("UnitScale", UnitScale);
            DA.GetData("AveragePrice", ref AveragePrice);
            DA.GetDataList("PriceWeight", PriceWeight);
            var Bricks = Util.BrickFromJson(FcD);
            var Total = Bricks[0] + Bricks[1] + Bricks[2];
            var Rate = Util.AdjustRatio(UnitScale);

            var EightUnit = Math.Round(Rate[0], 2) != 0 ? (int)Math.Round(Total * Rate[0] / 800.0) : 0;
            var FourUnit = Math.Round(Rate[1], 2) != 0 ? (int)Math.Round(Total * Rate[1] / 600.0) : 0;
            var TwoUnit = Math.Round(Rate[2], 2) != 0 ? (int)Math.Round(Total * Rate[2] / 200.0) : 0;

            if (EightUnit * 8 + FourUnit * 4 + TwoUnit * 2 < Total)
            {
                TwoUnit += (Total - (EightUnit * 8 + FourUnit * 4 + TwoUnit * 2)) / 2;
            }

            double EightPrice = AveragePrice * PriceWeight[0];
            double FourPrice = AveragePrice * PriceWeight[1];
            double TwoPrice = AveragePrice * PriceWeight[2];

            DA.SetData("FacadeTotalPrice",
                Math.Round(EightPrice * EightUnit + FourPrice * FourUnit + TwoPrice * TwoUnit));
            var JsonObj = new Dictionary<string, object>{
        {"TotalUnitCount", Total},
        {"Ratio", Rate},
        {"Price", new Dictionary<string, double>(){
            {"EightUnit_AveragePrice", EightPrice},
            {"FourUnit_AveragePrice", FourPrice},
            {"TwoUnit_AveragePrice", TwoPrice}
            }
          },
        {"UnitCount", new Dictionary<string, int>(){
            {"EightUnit", EightUnit},
            {"FourUnit", FourUnit},
            {"TwoUnit", TwoUnit}
            }}
        };
            var JsonString = Util.Serialise(JsonObj);
            DA.SetData("Data", JsonString);
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
            get { return new Guid("4120D85C-AC1E-4BCC-808B-7B23A2CF6466"); }
        }
    }
}