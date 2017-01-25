using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Office
{
    public class ColumnMapInfo
    {
        public string HeadText { get; set; }

        public bool HasBaseData { get; set; }

        public Dictionary<string, string> Values { get; set; }
    }
}
