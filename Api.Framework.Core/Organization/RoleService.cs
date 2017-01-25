using Api.Framework.Core.Config;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public class RoleService : IRole, IOrganization
    {
        private const string CONST_Origanization = "Origanization";    // 组织架构开头关键字
        private const string CONST_Role = "Role";               // 角色关键字
        private const string CONST_Permission = "Permission";   // 权限关键字
        private const string CONST_User = "User";

        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }
        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        public string CreateRole(string Name)
        {
            var keyPart = string.Format("{0}.{1}.", CONST_Origanization, CONST_Role);

            var configs = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(keyPart), keyPart);

            var maxVal = 0;
            configs.ForEach(c =>
            {
                var v = int.Parse(c.NodeName.Substring(1));
                if (v > maxVal)
                {
                    maxVal = v;
                }
            });

            var roleKey = string.Format("{0}R{1}", keyPart, maxVal + 1);

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = roleKey,
                Value = Name,
                IsDeleted = false,
                Type = "1"
            });

            return string.Format("R{0}", maxVal + 1);
        }

        public void AddPermission(int Index, long Value)
        {
            throw new NotImplementedException();
        }

        public void AddUser(int UserID)
        {
            throw new NotImplementedException();
        }

        public List<RoleInfo> All()
        {
            var deptPart = "Origanization.Role.";

            var configNodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPart), deptPart);

            var result = new List<RoleInfo>();

            convertToRole(configNodes, result);

            return result;
        }

        public void Delete(string Key)
        {
            var roleKey = "Origanization.Role." + Key;
            _IBaseConfig.Delete(roleKey);
        }

        public void Delete(int ID)
        {
            var config = _IBaseConfig.GetConfig(ID);
            _IBaseConfig.Delete(config.Key);
        }

        public Dictionary<int, long> GetRolePermissions(string RoleKey)
        {
            if (_AllRole == null)
            {
                _AllRole = this.All();
            }
            var role = _AllRole.FirstOrDefault(r => r.Key == RoleKey);
            if (role != null)
            {
                return toLongDic(role.PermissionValue);
            }

            return null;
        }

        private List<RoleInfo> _AllRole = null;

        public Dictionary<int, long> GetUserPermissions(string UserIdentity)
        {
            if (_AllRole == null)
            {
                _AllRole = this.All();
            }

            var rolePermissions = _AllRole.Where(r => r.Users != null ? r.Users.Select(u => u.ID.ToString()).Contains(UserIdentity) : false).Select(r => r.PermissionValue);

            var result = new Dictionary<int, long>();

            foreach (var item in rolePermissions)
            {
                result = _IPermissionOperate.Merge(result, toLongDic(item));
            }

            return result;
        }

        public Dictionary<int, long> toLongDic(Dictionary<int, string> Permissions)
        {
            if (Permissions == null)
            {
                return null;
            }

            var result = new Dictionary<int, long>();
            foreach (var p in Permissions)
            {
                result.Add(p.Key, long.Parse(p.Value));
            }

            return result;
        }

        public List<OrgInfo> GetUserRoles(int UserID)
        {
            if (_AllRole == null)
            {
                _AllRole = this.All();
            }

            return _AllRole.Where(r => r.Users != null ? r.Users.Select(u => u.ID).Contains(UserID) : false).Select(r => new OrgInfo() { Key = r.Key,Name = r.Name }).ToList();
        }

        public void RemovePermission(int Index, long Value)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(int UserID)
        {
            throw new NotImplementedException();
        }

        public void ReName(string Key, string Name)
        {
            var roleKey = "Origanization.Role." + Key;
            var config = _IBaseConfig.GetConfig(roleKey);

            config.Value = Name;

            _IBaseConfig.Update(config);
        }

        public void SetPermissions(string Key, string BusinessKey, Dictionary<int, long[]> Permission)
        {
            var permissionPart = string.Format("{0}.{1}.{2}.{3}.{4}.", CONST_Origanization, CONST_Role, Key, CONST_Permission, BusinessKey);

            var allPermissions = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(permissionPart));

            foreach (var permission in Permission)
            {
                var deptPart = string.Format("{0}{1}", permissionPart, permission.Key);

                var config = allPermissions.SingleOrDefault(c => c.Key.StartsWith(deptPart));

                if (config == null)
                {
                    // add
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = deptPart,
                        Value = permission.Value.Sum().ToString(),
                        Type = "1",
                        IsDeleted = false
                    });
                }
                else
                {
                    // update
                    config.Value = permission.Value.Sum().ToString();
                    _IBaseConfig.Update(config);
                }

            }

            allPermissions.ForEach(c =>
            {
                var index = int.Parse(c.Key.Replace(permissionPart, ""));

                if (!Permission.ContainsKey(index))
                {
                    // delete
                    _IBaseConfig.Delete(c.ID);
                }
            });
        }

        public void SetUsers(string Key, List<SysUserEntity> Users)
        {
            var userPart = string.Format("{0}.{1}.{2}.{3}.", CONST_Origanization, CONST_Role, Key, CONST_User);

            var allConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(userPart));

            Users.ForEach(u =>
            {
                var userKey = string.Format("{0}{1}", userPart, u.ID);

                var userConfig = allConfigs.SingleOrDefault(c => c.Key == userKey);

                if (userConfig == null)
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = userKey,
                        Value = u.Name,
                        IsDeleted = false,
                        Type = "1"
                    });
                }
                else if (!string.IsNullOrEmpty(u.Account) && userConfig.Value != u.Account)
                {
                    userConfig.Value = u.Account;
                    _IBaseConfig.Update(userConfig);
                }
            });

            allConfigs.ForEach(c =>
            {
                var userID = int.Parse(c.Key.Replace(userPart, ""));

                if (Users.SingleOrDefault(u => u.ID == userID) == null)
                {
                    _IBaseConfig.Delete(c.ID);
                }
            });
        }

        public void SetUserRoles(SysUserEntity User, List<OrgInfo> Roles)
        {
            var rolePart = string.Format("{0}.{1}", CONST_Origanization, CONST_Role);

            var userPart = string.Format("{0}.{1}", CONST_User, User.ID);

            var allConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(rolePart) && c.Key.EndsWith(userPart));

            var userRoleIDs = new List<string>();

            Roles.ForEach(r =>
            {
                var allPart = string.Format("{0}.{1}.{2}", rolePart, r.Key, userPart);

                userRoleIDs.Add(allPart);

                var role = allConfigs.FirstOrDefault(c => c.Key == allPart);

                if (role == null)
                {
                    // add
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = allPart,
                        Value = User.Name,
                        IsDeleted = false,
                        Type = "1"
                    });
                }
            });

            var qUserRoleIDs = userRoleIDs.AsQueryable();

            _IBaseConfig.Delete(c => c.Key.StartsWith(rolePart) && c.Key.EndsWith(userPart) && !qUserRoleIDs.Contains(c.Key));
        }

        #region 私有方法

        private void convertToRole(List<ConfigNode> nodes, List<RoleInfo> roles)
        {
            nodes.ForEach(n =>
            {
                var role = new RoleInfo()
                {
                    Key = n.NodeName,
                    Name = n.NodeValue,
                    PermissionValue = new Dictionary<int, string>()
                };

                n.ChildNodes.ForEach(cn =>
                {

                    switch (cn.NodeName)
                    {
                        case "Permission":
                            {
                                cn.ChildNodes.ForEach(buseinss =>
                                {
                                    buseinss.ChildNodes.ForEach(p =>
                                    {
                                        var index = int.Parse(p.NodeName);

                                        if (role.PermissionValue.ContainsKey(index))
                                        {
                                            role.PermissionValue[index] = _IPermissionOperate.Merge(long.Parse(role.PermissionValue[index]), long.Parse(p.NodeValue)).ToString();
                                        }
                                        else
                                        {
                                            role.PermissionValue.Add(index, p.NodeValue);
                                        }
                                    });
                                });

                                break;
                            }
                        case "User":
                            {
                                role.Users = new List<SysUserEntity>();

                                cn.ChildNodes.ForEach(u =>
                                {
                                    role.Users.Add(new SysUserEntity()
                                    {
                                        ID = int.Parse(u.NodeName),
                                        Account = u.NodeValue
                                    });
                                });

                                break;
                            }
                        default: break;
                    }
                });

                //role.Permissions = _IPermissionOperate.Prase(role.PermissionValue);

                roles.Add(role);
            });
        }

        #endregion
    }
}
