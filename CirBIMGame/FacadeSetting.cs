using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CirBIMGame
{
    public class FacadeSetting
    {
        private FacadeData[] _DataSetting;
        public FacadeSetting(List<Brep> facade)
        {
            int Index = 0;
            _DataSetting = facade.Select(x => { var Data = new FacadeData(x, Index); Index++; return Data; }).ToArray();
        }
        public List<string> Print()
        => this._DataSetting.Select(x => x.ToJson()).ToList();

        public void Setting(IEnumerable<string> DataSetting)
        {
            var Format = DataSetting.Select(x => JsonFormat.CreatFromJson(x)).ToList();
            foreach (var item in Format)
            {
                this._DataSetting[item.Index].SetRate(item.PatternRate);
            }
        }
    }
}
