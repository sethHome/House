using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Menu
{
    public class CreateMenuInfo
    {
        public string ParentKey { get; set; }

        public string BusinessKey { get; set; }

        public string Name { get; set; }

        public string Href { get; set; }

        public string Text { get; set; }

        public string Icon { get; set; }

        public string Param { get; set; }

        public string PermissionKey { get; set; }

        public int IndexValue { get; set; }
    }
}
