using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public interface IRole : IOrganization
    {
        List<RoleInfo> All();

        string CreateRole(string Name);

        Dictionary<int, long> GetRolePermissions(string RoleKey);

        Dictionary<int, long> GetUserPermissions(string UserIdentity);

        List<OrgInfo> GetUserRoles(int UserID);

        void SetUserRoles(SysUserEntity User, List<OrgInfo> Roles);
    }
}
