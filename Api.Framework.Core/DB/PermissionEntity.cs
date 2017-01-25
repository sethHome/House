using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class PermissionEntity
    {
        public int ID { get; set; }

        public int PID { get; set; }

        public int X { get; set; }

        public Int64 Y { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public int Type { get; set; }

        public bool CanInherit { get; set; }

        public string BusinessName { get; set; }
    }
}
