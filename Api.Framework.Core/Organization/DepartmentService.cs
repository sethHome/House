using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Framework.Core.Permission;

namespace Api.Framework.Core.Organization
{
    public class DepartmentService : IDepartment, IOrganization
    {
        private const string CONST_Origanization = "Origanization.Dept";    // 部门组织架构开头关键字
        private const string CONST_Dept = "Dept";               // 部门及子部门关键字
        private const string CONST_Permission = "Permission";   // 权限关键字
        private const string CONST_UnInheritPermission = "UnInheritPermission";     // 无法被继承权限关键字
        private const string CONST_User = "User";               // 部门用户关键字

        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }
        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        #region 接口方法

        public void AddPermission(int Index, long Value)
        {
            throw new NotImplementedException();
        }

        public void AddPosition(string Position)
        {
            throw new NotImplementedException();
        }

        public string AddDepartment(string Name, string ParentDeptKey = null)
        {
            var deptKeyPart = "";

            if (string.IsNullOrEmpty(ParentDeptKey))
            {
                deptKeyPart = CONST_Origanization;
            }
            else
            {
                deptKeyPart = string.Format("{0}.{1}.", ParentDeptKey, CONST_Dept);
            }

            var configs = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptKeyPart), deptKeyPart);

            var maxVal = 0;
            configs.ForEach(c =>
            {
                var v = int.Parse(c.NodeName.Substring(1));
                if (v > maxVal)
                {
                    maxVal = v;
                }
            });

            var deptKey = string.Format("{0}D{1}", deptKeyPart, maxVal + 1);

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = deptKey,
                Value = Name,
                IsDeleted = false,
                Type = "1"
            });

            return deptKey;

        }

        public void AddUser(int UserID)
        {
            throw new NotImplementedException();
        }

        public List<DepartmentInfo> All()
        {
            var deptPart = "Origanization.Dept.";

            var deptConfigNode = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPart), deptPart);

            var result = new List<DepartmentInfo>();

            convertToDept(deptConfigNode, result);

            return result;
        }

        public void Delete(string Key)
        {
            _IBaseConfig.Delete(Key);
        }

        public void Delete(int ID)
        {
            var config = _IBaseConfig.GetConfig(ID);
            _IBaseConfig.Delete(config.Key);
        }

        public List<Permission.Permission> GetDeptPermissions(string BusinessName, string DeptKey, bool inherit = true)
        {
            var deptPermissionKey = string.Format("{0}.Permission.", DeptKey);
            var deptPermission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPermissionKey), deptPermissionKey);

            var dicPermission = new Dictionary<int, long>();

            deptPermission.ForEach(p =>
            {
                dicPermission.Add(int.Parse(p.NodeName), long.Parse(p.NodeValue));
            });

            if (inherit)
            {
                // 继承权限
                var inheritPermission = GetInheritPermissions(DeptKey);

                var mergePermissoin = _IPermissionOperate.Merge(dicPermission, inheritPermission);

                return _IPermissionOperate.Prase(BusinessName, mergePermissoin, inheritPermission);
            }

            return _IPermissionOperate.Prase(BusinessName, dicPermission);
        }

        public List<Permission.Permission> GetDeptPermissions(string BusinessName, int DeptID, bool inherit = true)
        {
            var dept = _IBaseConfig.GetConfig(DeptID);
            var deptPermissionKey = string.Format("{0}.Permission.", dept.Key);
            var deptPermission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPermissionKey), deptPermissionKey);

            var dicPermission = new Dictionary<int, long>();

            deptPermission.ForEach(p =>
            {
                dicPermission.Add(int.Parse(p.NodeName), long.Parse(p.NodeValue));
            });

            if (inherit)
            {
                // 继承权限
                var inheritPermission = GetInheritPermissions(dept.Key);

                var mergePermissoin = _IPermissionOperate.Merge(dicPermission, inheritPermission);

                return _IPermissionOperate.Prase(BusinessName, mergePermissoin, inheritPermission);

            }

            return _IPermissionOperate.Prase(BusinessName, dicPermission);
        }

        public Dictionary<int, long> GetUserPermissions(string UserIdentity, bool inherit = true)
        {
            var dept = this.GetUserDept(int.Parse(UserIdentity)) as DepartmentInfo;

            if (dept == null)
            {
                return null;
            }

            if (inherit )
            {
                // 继承权限
                var inheritPermission = GetInheritPermissions(dept.Key);

                return _IPermissionOperate.Merge(toLongDic(dept.Permission), inheritPermission);
            }
            
            return toLongDic(dept.Permission);

            //var deptRootKey = "Origanization.Dept.";
            //var userDeptKey = ".User.";

            //var depts = _IBaseConfig.GetConfigEntitys(c =>
            //    c.Key.StartsWith(deptRootKey) && c.Key.EndsWith(userDeptKey + UserIdentity));

            //var dicPermission = new Dictionary<int, long>();

            //depts.ForEach(d =>
            //{
            //    var deptKey = d.Key.Split(new string[] { userDeptKey }, StringSplitOptions.None)[0];
            //    var deptPermissionKey = string.Format("{0}.Permission.", deptKey);

            //    var deptPermission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPermissionKey), deptPermissionKey);

            //    deptPermission.ForEach(p =>
            //    {
            //        var index = int.Parse(p.NodeName);
            //        var value = long.Parse(p.NodeValue);

            //        if (dicPermission.ContainsKey(index))
            //        {
            //            dicPermission[index] = _IPermissionOperate.Merge(dicPermission[index], value);
            //        }
            //        else
            //        {
            //            dicPermission.Add(index, value);
            //        }
            //    });

            //    if (inherit)
            //    {
            //        // 继承权限
            //        var inheritPermission = GetInheritPermissions(deptKey);

            //        dicPermission = _IPermissionOperate.Merge(dicPermission, inheritPermission);
            //    }

            //});

            //return dicPermission;
        }

        private List<DepartmentInfo> _UserDepts = null;

        public OrgInfo GetUserDept(int UserID)
        {
            if (_UserDepts == null)
            {
                var deptPart = "Origanization.Dept.";

                var deptConfigNode = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPart), deptPart);

                _UserDepts = new List<DepartmentInfo>();

                convertToDept2(deptConfigNode, _UserDepts);
            }

            var dept = _UserDepts.FirstOrDefault(d => d.Users.Select(u => u.ID).Contains(UserID));

            return dept;
        }

        public void RemovePermission(int Index, long Value)
        {
            throw new NotImplementedException();
        }

        public void RemovePosition(string Position)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(int UserID)
        {
            throw new NotImplementedException();
        }

        public void ReName(string Key, string Name)
        {
            var config = _IBaseConfig.GetConfig(Key);

            config.Value = Name;

            _IBaseConfig.Update(config);
        }

        public void SetUserDept(SysUserEntity User, string Key)
        {
            // 一个员工只能在一个部门下，删除该员工在其他部门的配置
            var otherDept = GetUserDept(User.ID);

            if (otherDept != null)
            {
                _IBaseConfig.Delete(string.Format("{0}.User.{1}", otherDept.Key, User.ID));
            }

            var userKey = string.Format("{0}.User.{1}", Key, User.ID);

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = userKey,
                Value = User.Name,
                IsDeleted = false,
                Type = "1"
            });
        }

        /// <summary>
        /// 更新部门下的员工
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Users"></param>
        public void SetUsers(string Key, List<SysUserEntity> Users)
        {
            var deptUserPart = string.Format("{0}.User.", Key);

            var allConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(deptUserPart));

            Users.ForEach(u =>
            {
                // 一个员工只能在一个部门下，删除该员工在其他部门的配置
                var otherDept = GetUserDept(u.ID);

                if (otherDept != null)
                {
                    _IBaseConfig.Delete(string.Format("{0}.User.{1}", otherDept.Key, u.ID));
                }

                var userKey = string.Format("{0}.User.{1}", Key, u.ID);

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
                //else if (!string.IsNullOrEmpty(u.Account) && userConfig.Value != u.Account)
                //{
                //    userConfig.Value = u.Account;
                //    _IBaseConfig.Update(userConfig);
                //}

            });

            allConfigs.ForEach(c =>
            {
                var userID = int.Parse(c.Key.Replace(deptUserPart, ""));

                if (Users.SingleOrDefault(u => u.ID == userID) == null)
                {
                    _IBaseConfig.Delete(c.ID);
                }
            });
        }
        /// <summary>
        /// 更新部门的权限
        /// </summary>
        /// <param name="DeptKey"></param>
        /// <param name="Permission"></param>
        public void SetPermissions(string DeptKey, string BusinessKey, Dictionary<int, long[]> Permission)
        {
            updateDeptPermission(DeptKey, BusinessKey, CONST_Permission, Permission);
        }
        /// <summary>
        /// 更新部门不允许被继承的权限
        /// </summary>
        /// <param name="DeptKey"></param>
        /// <param name="Permission"></param>
        public void SetUnInheritPermissions(string DeptKey, string BusinessKey, Dictionary<int, long[]> Permission)
        {
            updateDeptPermission(DeptKey, BusinessKey, CONST_UnInheritPermission, Permission);
        }

        /// <summary>
        /// 获取用户所属部门下所有的用户
        /// </summary>
        /// <param name="UserIdentity"></param>
        /// <returns></returns>
        public List<int> GetMyDeptUsers(int UserID)
        {
            var deptRootKey = "Origanization.Dept.";
            var userDeptKey = ".User.";

            var depts = _IBaseConfig.GetConfigEntitys(c =>
                c.Key.StartsWith(deptRootKey) && c.Key.EndsWith(userDeptKey + UserID));

            if (depts.Count > 0)
            {
                var deptKey = depts[0].Key.Split(new string[] { userDeptKey }, StringSplitOptions.None)[0];
                var deptUsers = string.Format("{0}.User.", deptKey);

                var deptConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(deptUsers));

                var result = new List<int>();

                foreach (var item in deptConfigs)
                {
                    result.Add(int.Parse(item.Key.Replace(deptUsers, "")));
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 私有方法

        private Dictionary<int, long> toLongDic(Dictionary<int, string> Permissions)
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

        private void updateDeptPermission(string DeptKey, string BusinessKey, string permissionType, Dictionary<int, long[]> Permission)
        {
            var permissionPart = string.Format("{0}.{1}.{2}", DeptKey, permissionType, BusinessKey);
            var allPermissions = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(permissionPart));

            foreach (var permission in Permission)
            {
                var deptPart = string.Format("{0}.{1}.{2}.{3}", DeptKey, permissionType, BusinessKey, permission.Key);

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

        private string getDeptFullKey(ConfigNode node)
        {
            if (node.ParentNode != null)
            {
                return string.Format("{0}.{1}", getDeptFullKey(node.ParentNode), node.NodeName);
            }
            else
            {
                return node.NodeName;
            }
        }

        private void convertToDept(List<ConfigNode> nodes, List<DepartmentInfo> depts)
        {
            nodes.ForEach(n =>
            {
                var dept = new DepartmentInfo()
                {
                    Key = string.Format("Origanization.Dept.{0}", getDeptFullKey(n)),
                    Name = n.NodeValue,
                    SubDepartments = new List<DepartmentInfo>()
                };

                // 部门权限
                var dicPer = new Dictionary<int, long>();

                n.ChildNodes.ForEach(cn =>
                {
                    switch (cn.NodeName)
                    {
                        case "Dept":
                            {
                                convertToDept(cn.ChildNodes, dept.SubDepartments);
                                break;
                            };
                        case "Permission":
                            {
                                cn.ChildNodes.ForEach(business =>
                                {
                                    business.ChildNodes.ForEach(p =>
                                    {
                                        var index = int.Parse(p.NodeName);

                                        if (dicPer.ContainsKey(index))
                                        {
                                            dicPer[index] = _IPermissionOperate.Merge(dicPer[index], long.Parse(p.NodeValue));
                                        }
                                        else
                                        {
                                            dicPer.Add(index, long.Parse(p.NodeValue));
                                        }

                                    });
                                });

                                break;
                            }
                        case "User":
                            {
                                dept.Users = new List<SysUserEntity>();

                                cn.ChildNodes.ForEach(u =>
                                {
                                    dept.Users.Add(new SysUserEntity()
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

                // 继承权限
                var inheritPermission = GetInheritPermissions(dept.Key);
                var mergePermission = _IPermissionOperate.Merge(dicPer, inheritPermission);
                var unInheritPermission = getDeptUnInheritPermission(dept.Key);

                dept.Permission = toStringDic(mergePermission);
                dept.InheritPermission = toStringDic(inheritPermission);
                dept.UnInheritPermission = toStringDic(unInheritPermission);

                //dept.Permissions = _IPermissionOperate.Prase(mergePermission, inheritPermission, unInheritPermission);

                depts.Add(dept);

            });
        }

        private void convertToDept2(List<ConfigNode> nodes, List<DepartmentInfo> depts)
        {
            nodes.ForEach(n =>
            {
                var dept = new DepartmentInfo()
                {
                    ID = n.ConfigID,
                    Key = string.Format("Origanization.Dept.{0}", getDeptFullKey(n)),
                    Name = n.NodeValue,
                    SubDepartments = new List<DepartmentInfo>(),
                    Users = new List<SysUserEntity>()
                };

                // 部门权限
                var dicPer = new Dictionary<int, long>();

                n.ChildNodes.ForEach(cn =>
                {
                    switch (cn.NodeName)
                    {
                        case "Dept":
                            {
                                convertToDept2(cn.ChildNodes, depts);
                                break;
                            };
                        case "Permission":
                            {
                                cn.ChildNodes.ForEach(business =>
                                {
                                    business.ChildNodes.ForEach(p =>
                                    {
                                        var index = int.Parse(p.NodeName);

                                        if (dicPer.ContainsKey(index))
                                        {
                                            dicPer[index] = _IPermissionOperate.Merge(dicPer[index], long.Parse(p.NodeValue));
                                        }
                                        else
                                        {
                                            dicPer.Add(index, long.Parse(p.NodeValue));
                                        }

                                    });
                                });

                                break;
                            }
                        case "User":
                            {

                                cn.ChildNodes.ForEach(u =>
                                {
                                    dept.Users.Add(new SysUserEntity()
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

                // 继承权限
                var inheritPermission = GetInheritPermissions(dept.Key);
                var mergePermission = _IPermissionOperate.Merge(dicPer, inheritPermission);
                var unInheritPermission = getDeptUnInheritPermission(dept.Key);

                dept.Permission = toStringDic(mergePermission);
                dept.InheritPermission = toStringDic(inheritPermission);
                dept.UnInheritPermission = toStringDic(unInheritPermission);

                //dept.Permissions = _IPermissionOperate.Prase(mergePermission, inheritPermission, unInheritPermission);

                depts.Add(dept);

            });
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

        /// <summary>
        /// 获取部门的继承权限
        /// </summary>
        /// <param name="deptKey"></param>
        /// <param name="permission"></param>
        public Dictionary<int, long> GetInheritPermissions(string DeptKey)
        {
            var parentKey = DeptKey.Substring(0, DeptKey.LastIndexOf(CONST_Dept) - 1);

            if (parentKey.Length > CONST_Origanization.Length)
            {
                // 获取可以被继承的权限
                var deptPermission = getDeptCanInheritPermission(parentKey);

                // 父级权限
                var parentPermission = GetInheritPermissions(parentKey);

                // 合并
                return _IPermissionOperate.Merge(parentPermission, deptPermission);
            }

            return null;
        }

        /// <summary>
        /// 获取部门可以被继承的权限
        /// </summary>
        /// <param name="deptKey"></param>
        /// <returns></returns>
        private Dictionary<int, long> getDeptCanInheritPermission(string deptKey)
        {
            var permission = new Dictionary<int, long>();

            var deptPermissionKey = string.Format("{0}.{1}.", deptKey, CONST_Permission);
            var deptPermission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptPermissionKey), deptPermissionKey);

            var unInheritPermissons = _IPermissionOperate.GetUnInheritPermissions();

            deptPermission.ForEach(business =>
            {
                business.ChildNodes.ForEach(p =>
                {
                    var index = int.Parse(p.NodeName);
                    var pvalue = long.Parse(p.NodeValue);

                    // 如果不能被继承的权限表中包含此维度，则把该部门此维度的权限中减去不能被继承的权限
                    if (unInheritPermissons.ContainsKey(index))
                    {
                        var value = _IPermissionOperate.Exclude(pvalue, unInheritPermissons[index]);

                        if (permission.ContainsKey(index))
                        {
                            permission[index] = _IPermissionOperate.Merge(permission[index], value);
                        }
                        else
                        {
                            permission.Add(index, value);
                        }
                    }
                    else
                    {
                        if (permission.ContainsKey(index))
                        {
                            permission[index] = _IPermissionOperate.Merge(permission[index], pvalue);
                        }
                        else
                        {
                            permission.Add(index, pvalue);
                        }
                    }
                });
            });

            // 这里排除掉不被继承的权限
            var unInheritPermission = getDeptUnInheritPermission(deptKey);
            _IPermissionOperate.Exclude(permission, unInheritPermission);

            return permission;
        }

        /// <summary>
        /// 获取本部门中不允许被继承的权限部分
        /// </summary>
        /// <param name="deptKey"></param>
        /// <returns></returns>
        private Dictionary<int, long> getDeptUnInheritPermission(string deptKey)
        {
            var deptUnInheritPermissionKey = string.Format("{0}.{1}.", deptKey, CONST_UnInheritPermission);
            var deptUnInheritPermission = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(deptUnInheritPermissionKey), deptUnInheritPermissionKey);

            var result = new Dictionary<int, long>();

            deptUnInheritPermission.ForEach(business =>
            {
                business.ChildNodes.ForEach(p =>
                {
                    var index = int.Parse(p.NodeName);
                    var value = long.Parse(p.NodeValue);

                    if (result.ContainsKey(index))
                    {
                        result[index] = _IPermissionOperate.Merge(result[index], value);
                    }
                    else
                    {
                        result.Add(index, value);
                    }
                });
            });

            return result;
        }


        #endregion
    }
}
