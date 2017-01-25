using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public class RoleInfo : OrgInfo
    {
        public List<SysUserEntity> Users { get; set; }

        public List<Permission.Permission> Permissions { get; set; }

        public Dictionary<int, string> PermissionValue { get; set; }

    }
}
