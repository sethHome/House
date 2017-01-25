using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public interface IDepartment : IOrganization
    {
        List<DepartmentInfo> All();

        string AddDepartment(string ParentDeptKey, string Name);

        void AddPosition(string Position);

        void RemovePosition(string Position);

        void SetUnInheritPermissions(string DeptKey, string BusinessKey, Dictionary<int, long[]> Permission);

        /// <summary>
        /// 获取部门的权限值
        /// </summary>
        /// <param name="DeptID"></param>
        /// <param name="inherit">是否需要继承的权限值</param>
        /// <returns></returns>
        List<Permission.Permission> GetDeptPermissions(string BusinessName,int DeptID,bool inherit = true);

        List<Permission.Permission> GetDeptPermissions(string BusinessName, string DeptKey, bool inherit = true);

        Dictionary<int, long> GetUserPermissions(string UserIdentity, bool inherit = true);

        OrgInfo GetUserDept(int UserID);

        List<int> GetMyDeptUsers(int UserID);

        Dictionary<int, long> GetInheritPermissions(string DeptKey);

        void SetUserDept(SysUserEntity User, string Key);
    }
}
