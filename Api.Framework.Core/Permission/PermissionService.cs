using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Organization;
using Api.Framework.Core.User;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public class PermissionService : BaseService<PermissionEntity>, IPermissionCheck, IPermissionPersistence
    {
        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        [Dependency]
        public IUser _IUser { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public IRole _IRole { get; set; }

        private const Int64 CONST_Max63Value = 4611686018427387904; // 63位
        private const Int64 CONST_Max62Value = 2305843009213693952; // 62位
        
        public List<Permission> All(Dictionary<string, object> filters)
        {
            Expression<Func<PermissionEntity, bool>> expression = c => true;

            foreach (var filter in filters)
            {
                switch (filter.Key)
                {
                    case "type":
                        {
                            var type = (int)filter.Value;
                            if (type > 0)
                            {
                                expression = expression.And(c => c.Type == type);
                            }
                            break;
                        }
                    case "business":
                        {
                            var business = filter.Value.ToString();
                            if (!string.IsNullOrEmpty(business))
                            {
                                expression = expression.And(c => c.BusinessName == business);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            var result = DB.GetList(expression).ToList();

            return _IPermissionOperate.To(result, 0);
        }

        public void Create(PermissionEntity Permission)
        {
            //var parent = base.DB.SingleOrDefault(c => c.ID == Permission.PID);

            //if (parent == null)
            //{
            //    throw new Exception("父级权限不存在，无法新增该权限");
            //}

            if (string.IsNullOrEmpty(Permission.Key))
            {
                Permission.Key = Guid.NewGuid().ToString();
            }

            var allPermissions = base.DB.All();
            var maxX = allPermissions.Max(p => p.X);
            var xPermissions = base.DB.GetQuery(p => p.X == maxX);
            var maxY = xPermissions.Max(p => p.Y);

            

            if (maxY == CONST_Max62Value)
            {
                Permission.X = maxX  + 1;
                Permission.Y = 1;
            }
            else
            {
                Permission.X = maxX;
                Permission.Y = maxY * 2;
            }

            base.DB.Add(Permission);
        }

        public void Delete(string Key)
        {
            throw new NotImplementedException();
        }

        public void Delete(int ID)
        {
            var child = base.DB.GetQuery(p => p.PID == ID).ToList();

            child.ForEach(c =>
            {
                this.Delete(c.ID);
            });

            base.DB.Delete(ID);
        }

        public void Update(PermissionEntity Permission)
        {
            var dbPermission = base.DB.Get(Permission.ID);

            dbPermission.Key = Permission.Key;
            dbPermission.Name = Permission.Name;
            dbPermission.CanInherit = Permission.CanInherit;

            base.DB.Edit(dbPermission);
        }

        // 用户 部门 角色权限缓存
        private Dictionary<string, Dictionary<int, long>> _UserPermissionCache = new Dictionary<string, Dictionary<int, long>>() ;
        private Dictionary<string, Dictionary<int, long>> _UserDeptPermissionCache = new Dictionary<string, Dictionary<int, long>>();
        private Dictionary<string, Dictionary<int, long>> _UserRolePermissionCache = new Dictionary<string, Dictionary<int, long>>();

        public PermissionStatus Check(string PermissionKey, string UserIdentity,string Business)
        {
            var permission = base.DB.SingleOrDefault(c => c.Key == PermissionKey && c.BusinessName == Business);

            if (permission == null)
            {
                // 权限未定义
                return PermissionStatus.Undefined;
            }

            // 权限验证 这里暂时没有做父级权限的递归验证
            //if (permission.PID > 0)
            //{
            //}

            // 验证用户自身权限
            if (!_UserPermissionCache.ContainsKey(UserIdentity))
            {
                var userPermission = _IUser.GetPermissions(UserIdentity);
                _UserPermissionCache.Add(UserIdentity, userPermission);
            }

            if (_UserPermissionCache[UserIdentity].ContainsKey(permission.X) &&
                _IPermissionOperate.Contains(_UserPermissionCache[UserIdentity][permission.X], permission.Y))
            {
                return PermissionStatus.Confer;
            }

            // 验证角色权限
            if (!_UserRolePermissionCache.ContainsKey(UserIdentity))
            {
                var userRolePermission = _IRole.GetUserPermissions(UserIdentity);
                _UserRolePermissionCache.Add(UserIdentity, userRolePermission);
            }

            if (_UserRolePermissionCache[UserIdentity].ContainsKey(permission.X) &&
                _IPermissionOperate.Contains(_UserRolePermissionCache[UserIdentity][permission.X], permission.Y))
            {
                return PermissionStatus.InheritRole;
            }

            // 验证部门权限
            if (!_UserDeptPermissionCache.ContainsKey(UserIdentity))
            {
                var userDeptPermission = _IDepartment.GetUserPermissions(UserIdentity);
                _UserDeptPermissionCache.Add(UserIdentity, userDeptPermission);
            }

            if (_UserDeptPermissionCache[UserIdentity].ContainsKey(permission.X) &&
                _IPermissionOperate.Contains(_UserDeptPermissionCache[UserIdentity][permission.X], permission.Y))
            {
                return PermissionStatus.InheritDept;
            }

            return PermissionStatus.Reject;
        }
    }
}
