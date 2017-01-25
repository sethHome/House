using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BaseData
{
    public class EnumItemInfo
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }

        public bool GrowBinary { get; set; }

        public Dictionary<string, string> Tags { get; set; }
    }
}
