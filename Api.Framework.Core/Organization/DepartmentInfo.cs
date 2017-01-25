using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public class DepartmentInfo : OrgInfo
    {
        public List<SysUserEntity> Users { get; set; }

        public Dictionary<int, string> Permission { get; set; }

        public Dictionary<int, string> InheritPermission { get; set; }

        public Dictionary<int, string> UnInheritPermission { get; set; }

        public List<Permission.Permission> Permissions { get; set; }

        public List<DepartmentInfo> SubDepartments { get; set; }
    }
}
