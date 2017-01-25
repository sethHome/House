using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Organization
{
    public class DepartmentPermission
    {
        public Dictionary<int, long[]> Permissions { get; set; }

        public Dictionary<int, long[]> UnInheritPermissions { get; set; }
    }
}
