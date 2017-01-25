using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BaseData
{
    public class EnumInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public List<EnumItemInfo> Items { get; set; }
    }
}
