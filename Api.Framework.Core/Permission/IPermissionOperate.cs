using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public interface IPermissionOperate
    {
        /// <summary>
        /// 将权限字典转换为权限树
        /// </summary>
        /// <param name="Permissions"></param>
        /// <returns></returns>
        List<Permission> Prase(string BusinessName,Dictionary<int, long> Permissions, Dictionary<int, long> InheritPermissions = null, Dictionary<int, long> UnInheritPermissions = null);

        Dictionary<int, long> Prase(List<Permission> Permissions);

        List<Permission> To(IEnumerable<PermissionEntity> permissions, int pID, Dictionary<int, long> InheritPermissions = null, Dictionary<int, long> UnInheritPermissions = null);

        /// <summary>
        /// 从权限集合A中排除掉所有权限B中的值
        /// </summary>
        /// <param name="PermissionsA"></param>
        /// <param name="PermissionsB"></param>
        /// <returns></returns>
        void Exclude(Dictionary<int, long> PermissionsA, Dictionary<int, long> PermissionsB);

        /// <summary>
        /// 从权限A中排除权限B
        /// </summary>
        /// <param name="PermissionA"></param>
        /// <param name="PermissionB"></param>
        long Exclude(long PermissionA, long PermissionB);

        /// <summary>
        /// 合并权限
        /// </summary>
        /// <param name="PermissionsA"></param>
        /// <param name="PermissionsB"></param>
        Dictionary<int, long>  Merge(Dictionary<int, long> PermissionsA, Dictionary<int, long> PermissionsB);

        long Merge(long PermissionsA, long PermissionsB);

        /// <summary>
        /// 获取不能被继承的权限
        /// </summary>
        /// <returns></returns>
        Dictionary<int, long> GetUnInheritPermissions();

        /// <summary>
        /// 判断权限A中是否包含权限B
        /// </summary>
        /// <param name="PermissionsA"></param>
        /// <param name="PermissionsB"></param>
        /// <returns></returns>
        bool Contains(long PermissionsA, long PermissionsB);
    }
}
