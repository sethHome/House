using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Config
{
    public class ConfigNode
    {
        public int ConfigID { get; set; }

        public string NodeName { get; set; }

        public string NodeValue { get; set; }

        public int Deep { get; set; }

        public string Type { get; set; }

        public string Tag { get; set; }

        public List<ConfigNode> ChildNodes { get; set; }

        public ConfigNode ParentNode { get; set; }

        public Dictionary<string,string> Propertys { get; set; }
    }
}
