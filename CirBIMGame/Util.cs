using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CirBIMGame
{
    public static class Util
    {
        public static int[] BrickFromJson(IEnumerable<string> FcD)
        {
            int bluebrickAmount = 0;
            int yellowbrickAmount = 0;
            int normalbrickAmount = 0;
            foreach (var item in FcD)
            {
                var FcDData = JsonConvert.DeserializeObject<Dictionary<string, object>>(item);
                var BrickData = JsonConvert.DeserializeObject<Dictionary<string, int>>(FcDData["BrickPatternNumber"].ToString());
                bluebrickAmount += BrickData["bluebrick"];
                yellowbrickAmount += BrickData["yellowbrick"];
                normalbrickAmount += BrickData["normalbrick"];
            }
            return new int[3] { bluebrickAmount, yellowbrickAmount, normalbrickAmount };
        }
        public static double[] AdjustRatio(IEnumerable<double> Ratio)
        {
            var RatioArr = Ratio.ToArray();
            if (Ratio.ToArray().Length != 3)
                return new double[3] { 0, 0, 100 };
            else if (RatioArr[0] + RatioArr[1] + RatioArr[2] > 100)
                return new double[3] { 0, 0, 100 };
            else if (RatioArr[0] + RatioArr[1] + RatioArr[2] == 100)
                return RatioArr;
            else
            {
                int IsZero = 0;
                var Result = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    if (RatioArr[i] != 0)
                        Result[i] = RatioArr[i];
                    else
                        IsZero++;
                }
                if (IsZero == 0 || ((IsZero == 1 || IsZero == 2) && RatioArr[2] == 0))
                    Result[2] = 100 - RatioArr[0] - RatioArr[1];
                else if (IsZero == 1 || IsZero == 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (RatioArr[i] == 0)
                        {
                            Result[i] = 100 - RatioArr[(i + 1) % 3] - RatioArr[(i + 2) % 3];
                            break;
                        }
                    }
                }
                else
                    Result = new double[3] { 0, 0, 100 };
                return Result;
            }

        }
        public static string Serialise(object obj)
        {
            if (obj == null)
                return "";
            else
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        public static T Deserialiser<T>(string Json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Json);
        }
        public static double[] RateCalculator(double[] Rates)
        {
            var Result = new double[4];
            var BoolPattern = new bool[4]
            {IsZero(Rates[0]), IsZero(Rates[1]), IsZero(Rates[2]), IsZero(Rates[3])};
            int ValueAmount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!BoolPattern[i])
                    Result[i] = Rates[i];
                else
                    ValueAmount++;
            }

            if (ValueAmount == 4 ||
            ((ValueAmount == 3 ||
            ValueAmount == 1) && BoolPattern[3]) ||
            ValueAmount == 0)
                Result[3] = 100 - (Result[2] + Result[1] + Result[0]);
            else if (ValueAmount == 3 || (ValueAmount == 2 && BoolPattern[3]))
                Result[3] = 100 - (Result[2] + Result[1] + Result[0]);
            else if ((ValueAmount == 2 && !BoolPattern[3]) || (ValueAmount == 1))
            {

                for (int i = 0; i < 4; i++)
                {
                    if (BoolPattern[i])
                    {
                        Result[i] = 100 - (Result[(i + 1) % 4] + Result[(i + 2) % 4] + Result[(i + 3) % 4]);
                        break;
                    }
                }
            }

            return Result;
            bool IsZero(double Number) => Number == 0;
        }
    }
}
