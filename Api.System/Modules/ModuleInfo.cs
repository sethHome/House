using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Modules
{
    public class ModuleInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Src { get; set; }

        public string Text { get; set; }

        public string Param { get; set; }

        public string System { get; set; }

        public bool Tab { get; set; }

        public List<ModuleInfo> SubModules { get; set; }
    }
}
