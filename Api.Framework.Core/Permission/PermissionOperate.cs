using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public  class PermissionOperate : BaseService<PermissionEntity>, IPermissionOperate
    {
        /// <summary>
        /// 将权限字典转换为权限树
        /// </summary>
        /// <param name="Permissions"></param>
        /// <returns></returns>
        public  List<Permission> Prase(string BusinessName, Dictionary<int, long> Permissions, 
            Dictionary<int, long> InheritPermissions = null, 
            Dictionary<int, long> UnInheritPermissions = null)
        {
            var ps = new List<PermissionEntity>();

            foreach (var permission in Permissions)
            {
                ps.AddRange(DB.GetList(p =>
                    p.BusinessName == BusinessName &&
                    p.X == permission.Key && 
                    (p.Y & permission.Value) > 0));
            }

            return To(ps, 0, InheritPermissions, UnInheritPermissions);
        }

        public  Dictionary<int, long> Prase(List<Permission> Permissions)
        {
            var dicPermission = new Dictionary<int, long>();

            Permissions.ForEach(u =>
            {
                if (dicPermission.ContainsKey(u.Index))
                {
                    dicPermission[u.Index] = dicPermission[u.Index] + u.Value;
                }
                else
                {
                    dicPermission.Add(u.Index, u.Value);
                }
            });

            return dicPermission;
        }

        /// <summary>
        /// 将配置树转换为权限树
        /// </summary>
        /// <param name="configNodes"></param>
        /// <returns></returns>
        public  List<Permission> To(IEnumerable<PermissionEntity> permissions, int pID, 
            Dictionary<int, long> InheritPermissions = null, 
            Dictionary<int, long> UnInheritPermissions = null)
        {
            var result = new List<Permission>();

            var ps = permissions.Where(p => p.PID == pID);

            foreach (var p in ps)
            {
                result.Add(new Permission()
                {
                    ID = p.ID,
                    Key = p.Key,
                    Name = p.Name,
                    Value = p.Y,
                    StrValue = p.Y.ToString(),
                    Index = p.X,
                    CanInherit = p.CanInherit,
                    BusinessName = p.BusinessName,
                    OrgCanInherit = !(UnInheritPermissions != null && UnInheritPermissions.ContainsKey(p.X) && (UnInheritPermissions[p.X] & p.Y) > 0),
                    IsInherit = InheritPermissions != null && InheritPermissions.ContainsKey(p.X) && (InheritPermissions[p.X] & p.Y) > 0,
                    Children = To(permissions, p.ID, InheritPermissions, UnInheritPermissions)
                });
            }

            return result;
        }

        /// <summary>
        /// 从权限集合A中排除掉所有权限B中的值
        /// </summary>
        /// <param name="PermissionsA"></param>
        /// <param name="PermissionsB"></param>
        /// <returns></returns>
        public  void Exclude(Dictionary<int, long> PermissionsA, Dictionary<int, long> PermissionsB)
        {
            var dicTemp = new Dictionary<int, long>();

            foreach (var itemA in PermissionsA)
            {
                if (PermissionsB.ContainsKey(itemA.Key))
                {
                    dicTemp.Add(itemA.Key, Exclude(itemA.Value, PermissionsB[itemA.Key]));
                }
            }

            foreach (var item in dicTemp)
            {
                PermissionsA[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// 从权限A中排除权限B
        /// </summary>
        /// <param name="PermissionA"></param>
        /// <param name="PermissionB"></param>
        public  long Exclude(long PermissionA, long PermissionB)
        {
            if ((PermissionA & PermissionB) == 0)
            {
                // A: 101 B: 010 = 101
                return PermissionA;
            }
            else if((PermissionA ^ PermissionB) > PermissionA)
            {
                // A: 101 B: 1001  = 100
                return PermissionA & PermissionB ^ PermissionA;
            }
            else 
            {
                // A: 101 B: 001 = 100
                return PermissionA ^ PermissionB;
            }
        }

        /// <summary>
        /// 合并权限
        /// </summary>
        /// <param name="PermissionsA"></param>
        /// <param name="PermissionsB"></param>
        public Dictionary<int, long> Merge(Dictionary<int, long> PermissionsA, Dictionary<int, long> PermissionsB)
        {
            if (PermissionsA == null && PermissionsB != null)
            {
                return PermissionsB;
            }
            else if (PermissionsB == null && PermissionsA != null)
            {
                return PermissionsA;
            }
            else if (PermissionsB == null && PermissionsA == null)
            {
                return null;
            }

            var dicTemp = new Dictionary<int, long>();

            foreach (var itemA in PermissionsA)
            {
                dicTemp.Add(itemA.Key, itemA.Value);
            }

            foreach (var itemB in PermissionsB)
            {
                if (dicTemp.ContainsKey(itemB.Key))
                {
                    dicTemp[itemB.Key] = Merge(itemB.Value, PermissionsA[itemB.Key]);
                }
                else
                {
                    dicTemp.Add(itemB.Key, itemB.Value);
                }
            }

            return dicTemp;

        }

        /// <summary>
        /// 合并权限A和B
        /// </summary>
        /// <param name="PermissionA"></param>
        /// <param name="PermissionB"></param>
        public long Merge(long PermissionA, long PermissionB)
        {
            return PermissionA | PermissionB;
        }

        /// <summary>
        /// 获取不能被继承的权限
        /// </summary>
        /// <returns></returns>
        public  Dictionary<int, long> GetUnInheritPermissions()
        {
            var unInheritPermissions = DB.GetList(p => !p.CanInherit);

            var dicPermissions = new Dictionary<int, long>();

            foreach (var item in unInheritPermissions)
            {
                if (dicPermissions.ContainsKey(item.X))
                {
                    dicPermissions[item.X] = Merge(dicPermissions[item.X], item.Y);
                }
                else
                {
                    dicPermissions.Add(item.X, item.Y);
                }
            }

            return dicPermissions;
        }

        public bool Contains(long PermissionsA, long PermissionsB)
        {
            return (PermissionsA & PermissionsB) > 0;
        }
    }
}
