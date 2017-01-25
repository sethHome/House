using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Config;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public class UserService : BaseService<SysUserEntity>, IUser
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public IRole _IRole { get; set; }

        [Dependency]
        public IBusinessSystem _IBusinessSystem { get; set; }

        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        public Dictionary<int, long> GetPermissions(string UserIdentity)
        {
            var permissionKey = string.Format("User.{0}.Permission.", UserIdentity);

            var permission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(permissionKey), permissionKey);

            var dicPermission = new Dictionary<int, long>();

            permission.ForEach(business =>
            {
                business.ChildNodes.ForEach(p =>
                {
                    var index = int.Parse(p.NodeName);

                    if (dicPermission.ContainsKey(index))
                    {
                        dicPermission[index] = _IPermissionOperate.Merge(dicPermission[index], long.Parse(p.NodeValue));
                    }
                    else
                    {
                        dicPermission.Add(index, long.Parse(p.NodeValue));
                    }
                });
            });

            return dicPermission;
        }

        public List<UserInfo> GetUsers(bool WithDept = false, bool WithRole = false, bool WithPermission = false, bool WithBusiness = false)
        {
            var users = base.DB.All().ToList();

            var result = new List<UserInfo>();

            users.ForEach(u =>
            {
                result.Add(new UserInfo()
                {
                    ID = u.ID,
                    Account = u.Account,
                    Name = u.Name,
                    PhotoImg = u.PhotoImg,
                    PhotoImgLarge = u.PhotoImgLarge,
                    Visiable = u.Visiable,

                    Dept = WithDept ? _IDepartment.GetUserDept(u.ID) : null,
                    Roles = WithRole ? _IRole.GetUserRoles(u.ID) : null,
                    Businesses = WithBusiness ? _IBusinessSystem.GetBusiness(u.ID) : null,

                    PermissionsValue = WithPermission ? toStringDic(GetPermissions(u.ID.ToString())) : null,
                    DeptPermissionsValue = WithPermission ? toStringDic(_IDepartment.GetUserPermissions(u.ID.ToString())) : null,
                    RolePermissionsValue = WithPermission ? toStringDic(_IRole.GetUserPermissions(u.ID.ToString())) : null,
                    
                });
            });

            return result.OrderBy(u => u.Dept == null ? "z": u.Dept.Key).ToList();
        }

        public Dictionary<int, string> toStringDic(Dictionary<int, long> Permissions)
        {
            if (Permissions == null)
            {
                return null;
            }

            var result = new Dictionary<int, string>();
            foreach (var p in Permissions)
            {
                result.Add(p.Key, p.Value.ToString());
            }

            return result;
        }

        public List<SysUserEntity> GetBaseUsers()
        {
            return base.DB.GetList(u => u.Visiable).ToList();
        }

        public void SetPermission(int UserID, string BusinessKey, Dictionary<int, long[]> Permission)
        {
            foreach (var p in Permission)
            {
                var permissionKey = string.Format("User.{0}.Permission.{1}.{2}", UserID, BusinessKey, p.Key);

                var config = _IBaseConfig.GetConfig(permissionKey);

                if (config == null)
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = permissionKey,
                        Value = p.Value.Sum().ToString(),
                        IsDeleted = false,
                        Type = "1"
                    });
                }
                else if (config.Value != p.Value.ToString())
                {
                    // | === + ？,如果用户自身的权限有问题，应该检查此处
                    config.Value = p.Value.Sum().ToString();

                    _IBaseConfig.Update(config);
                }
            }

            var userPermissionKey = string.Format("User.{0}.Permission.{1}.", UserID,BusinessKey);
            var allConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(userPermissionKey));

            foreach (var config in allConfigs)
            {
                var index = int.Parse(config.Key.Replace(userPermissionKey, ""));

                if (!Permission.ContainsKey(index))
                {
                    _IBaseConfig.Delete(config.ID);
                }
            }
        }

        public int ChangePassword(int ID, UserPasswordInfo PaswordInfo)
        {
            var user = base.DB.Get(ID);

            var md5Password = Md5.GetMd5Hash(PaswordInfo.OldPassword);

            if (user.Password != md5Password.ToUpper())
            {
                return -1; // 原密码不正确
            }

            user.Password = Md5.GetMd5Hash(PaswordInfo.NewPassword).ToUpper();

            base.DB.Edit(user);

            return 1;
        }

        public void SetFlowTask(int ID, UserProductionInfo Info)
        {
            var key = string.Format("User.{0}.Specialty.{1}.Process.{2}.Tasks", ID, Info.SpeciltyID, Info.ProcessKey);

            var config = _IBaseConfig.GetConfig(key);

            if (config == null)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = key,
                    Value = Info.TaskIDs,
                    IsDeleted = false,
                    Type = "1"
                });
            }
            else 
            {
                config.Value = Info.TaskIDs;

                _IBaseConfig.Update(config);
            }
        }

        public void SetUserPhotot(int ID, SysUserEntity UserInfo)
        {
            var user = base.DB.Get(ID);
            user.PhotoImg = UserInfo.PhotoImg;
            user.PhotoImgLarge = UserInfo.PhotoImgLarge;
            base.DB.Edit(user);
        }

        public void ResetUserPassword(int ID)
        {
            var user = base.DB.Get(ID);
            user.Password = Md5.GetMd5Hash("pass").ToUpper(); 
            base.DB.Edit(user);
        }

        public void DisableUser(int ID)
        {
            var user = base.DB.Get(ID);
            user.Visiable = false;
            base.DB.Edit(user);
        }

        public void EnableUser(int ID)
        {
            var user = base.DB.Get(ID);
            user.Visiable = true;
            base.DB.Edit(user);
        }

        public void Update(int ID,UserInfo User)
        {
            var user = base.DB.Get(ID);
            user.Name = User.Name;
            user.Account = user.Account;
            base.DB.Edit(user);

            if (User.Dept != null && !string.IsNullOrEmpty(User.Dept.Key))
            {
                // 更新用户所属部门
                _IDepartment.SetUserDept(user, User.Dept.Key);
            }

            if (User.Roles != null)
            {
                _IRole.SetUserRoles(user, User.Roles);
            }

            if (User.Businesses != null)
            {
                _IBusinessSystem.SetUserBusinesses(user, User.Businesses);
            }
        }

        public int Create(UserInfo User)
        {
            var newUser = new SysUserEntity()
            {
                Account = User.Account,
                Name = User.Name,
                Password = Md5.GetMd5Hash("pass"),
                PhotoImg = "1.jpg",
                PhotoImgLarge = "1x.jpg",
                Visiable = true
            };

            base.DB.Add(newUser);

            if (User.Dept != null && !string.IsNullOrEmpty(User.Dept.Key))
            {
                _IDepartment.SetUserDept(newUser, User.Dept.Key);
            }

            if (User.Roles != null)
            {
                _IRole.SetUserRoles(newUser, User.Roles);
            }

            if (User.Businesses != null)
            {
                _IBusinessSystem.SetUserBusinesses(newUser, User.Businesses);
            }

            return newUser.ID;
        }

        public bool CheckAccount(string Account)
        {
            return DB.Count(u => u.Account == Account) == 0;
        }
    }
}
