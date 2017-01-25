using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Modules
{
    public class CreateModuleInfo
    {
        public string ParentKey { get; set; }

        public string BusinessKey { get; set; }

        public string Name { get; set; }

        public string Src { get; set; }

        public string Param { get; set; }

        public string Text { get; set; }

        public bool Tab { get; set; }
    }
}
