using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grasshopper;
using Rhino.Geometry;

namespace CirBIMGame
{
    public class JsonFormat
    {
        public int Index { get; set; }
        public double Area { get; set; }
        public int BrickCount { get; set; }
        public Dictionary<string, double> PatternRate { get; set; }
        public Dictionary<string, int> BrickPatternNumber { get; set; }
        private JsonFormat()
        {
            PatternRate = new Dictionary<string, double>()
        {
            {"voidrate", 0},
            {"bluebrick", 0},
            {"yellowbrick", 0},
            {"normalbrick", 0}
        };
            BrickPatternNumber = new Dictionary<string, int>(){
            {"voidrate", 0},
            {"bluebrick", 0},
            {"yellowbrick", 0},
            {"normalbrick", 0}
        };
        }
        public static JsonFormat CreatFromJson(string Json)
        {
            Dictionary<string, object> Data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Json);


            var JsonForm = new JsonFormat();

            if (!Data.ContainsKey("Index"))
                throw new Exception("No index setting");

            foreach (var item in Data)
            {
                switch (item.Key)
                {
                    case "Index":
                        JsonForm.Index = int.Parse(item.Value.ToString());
                        break;
                    case "Area":
                        JsonForm.Area = double.Parse(item.Value.ToString());
                        break;
                    case "BrickCount":
                        JsonForm.BrickCount = int.Parse(item.Value.ToString());
                        break;
                    case "PatternRate":
                        var PatDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, double>>(item.Value.ToString());
                        JsonForm.PatternRate = PatDic;
                        break;
                    case "BrickPatternNumber":
                        var BriDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(item.Value.ToString());
                        JsonForm.BrickPatternNumber = BriDic;
                        break;
                    default:
                        throw new Exception("Label cannot be recognised");
                }
            }

            return JsonForm;
        }
        public JsonFormat(FacadeData Data)
        {
            this.Index = Data.Index;
            this.Area = Data.Area;
            this.BrickCount = Data.BrickCount;
            this.PatternRate = Data._PatternRate;
            this.BrickPatternNumber = Data.BrickPatternNumber;
        }
        public string ToJson()
         => Newtonsoft.Json.JsonConvert.SerializeObject(this, Formatting.Indented);
        public static JsonFormat Set(int _Index, Dictionary<string, double> _PatternRate)
        {
            return new JsonFormat() { Index = _Index, PatternRate = _PatternRate };
        }
    }
}
