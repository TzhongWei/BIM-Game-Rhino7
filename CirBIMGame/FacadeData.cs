using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CirBIMGame
{
    public class FacadeData
    {
        public string ToJson()
            => (new JsonFormat(this)).ToJson();

        public static double UnitSize = 250;
        private Brep Face;
        public double Area => Face.GetArea();
        public Dictionary<string, double> _PatternRate { get; private set; }
        public Dictionary<string, int> BrickPatternNumber
        {
            get
            {
                var brickPatternCount = new Dictionary<string, int>();
                int Total = 0;
                foreach (var Kvp in _PatternRate)
                {
                    if (Kvp.Key == "normalbrick") continue;
                    var Number = (int)Math.Round(BrickCount * Kvp.Value / 100);
                    brickPatternCount.Add(Kvp.Key, Number);
                    Total += Number;
                }
                brickPatternCount["normalbrick"] = BrickCount - Total;
                return brickPatternCount;
            }
        }
        public int BrickCount { get; private set; }
        public int Index { get; }
        public FacadeData(Brep face, int index = 0)
        {
            this.Index = index;
            Face = face;
            this.BrickCount = _BrickCount();
            _PatternRate = new Dictionary<string, double>();
            _PatternRate.Add("voidrate", 0);
            _PatternRate.Add("bluebrick", 0);
            _PatternRate.Add("yellowbrick", 0);
            _PatternRate.Add("normalbrick", 100);
        }
        public void SetRate(Dictionary<string, double> PatternRate)
        {
            this._PatternRate = PatternRate;
        }
        public void SetRate(string Name, double Rate)
        {
            if (_PatternRate.ContainsKey(Name.ToLower()))
            {
                if (Rate >= 100)
                    throw new Exception("Rate Setting invalid");
                else if (this._PatternRate[Name.ToLower()] != 0)
                {
                    var OldValue = this._PatternRate[Name.ToLower()];
                    this._PatternRate[Name.ToLower()] = 0;
                    this._PatternRate["normalbrick"] += OldValue;
                    this.SetRate(Name.ToLower(), Rate);
                }
                else if (RateCorrectSetting(Rate))
                {
                    this._PatternRate[Name.ToLower()] = Rate;
                    this._PatternRate["normalbrick"] -= Rate;
                }
                else
                    throw new Exception("Rate Setting invalid. Some parameter need to be reset.");
            }
            else
                throw new Exception("The provided label isn't existed");
            bool RateCorrectSetting(double iRate)
            {
                if (this._PatternRate["normalbrick"] - iRate < 0)
                    return false;
                var Values = this._PatternRate.Values.ToList();
                double Final = 0;
                for (int i = 0; i < Values.Count; i++)
                {
                    Final += Values[i];
                }
                Final += iRate;
                return iRate > 100 ? false : true;
            }
        }
        private int _BrickCount()
            => (int)Math.Round(Area / (UnitSize * UnitSize));
    }
}
