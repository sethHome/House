using Api.Framework.Core;
using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Config;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Menu
{
    public class MenuService : IMenu
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IPermissionCheck _IPermission { get; set; }

        [Dependency]
        public IBusinessSystem _IBusinessSystem { get; set; }

        /// <summary>
        /// 保留的关键字
        /// </summary>
        private string[] _DisableKey = new string[] { "src", "Menus" };

        public List<MenuInfo> GetMenus(string BusinessKey, string UserIdentity)
        {
            var moduleKey = string.Format("BusinessSystem.{0}.Menus.", BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(moduleKey) && !c.IsDeleted, moduleKey);

            return convert(nodes, string.Format("BusinessSystem.{0}", BusinessKey), UserIdentity, BusinessKey);

        }

        public List<BusinessMenuInfo> GetAllMenus(string UserIdentity)
        {
            var result = new List<BusinessMenuInfo>();

            var syss = _IBusinessSystem.GetBusiness(int.Parse(UserIdentity));

            //var sysKeys = _IBusinessSystem.All().Select(c => c.Key);

            foreach (var s in syss)
            {
                result.Add(new BusinessMenuInfo()
                {
                    Key = s.Key,
                    Name = s.Name,
                    Menus = this.GetMenus(s.Key, UserIdentity)
                });
            }

            return result;
        }

        public void AddMenu(CreateMenuInfo Menu)
        {
            var key = "";

            if (!string.IsNullOrEmpty(Menu.ParentKey))
            {
                key = string.Format("{0}.Menus.{1}", Menu.ParentKey, Menu.Name);
            }
            else if (!string.IsNullOrEmpty(Menu.BusinessKey))
            {
                key = string.Format("BusinessSystem.{0}.Menus.{1}", Menu.BusinessKey, Menu.Name);
            }
            else
            {
                throw new Exception("必须指定业务系统或者父级模块的Key");
            }

            if (_DisableKey.Contains(Menu.Name))
            {
                throw new Exception(string.Format("使用了保留关键字：{0}", Menu.Name));
            }

            if (string.IsNullOrEmpty(Menu.Name))
            {
                throw new Exception("菜单Key不能为空");
            }

            var module = new ConfigEntity()
            {
                Key = key,
                Value = Menu.Text,
                IsDeleted = false,
                Type = "1"
            };
            _IBaseConfig.Add(module);

            addMenuProperty(key, "href", Menu.Href);
            addMenuProperty(key, "icon", Menu.Icon);
            addMenuProperty(key, "param", Menu.Param);
            addMenuProperty(key, "permission", Menu.PermissionKey);
            addMenuProperty(key, "index", Menu.IndexValue.ToString());

        }

        public void Update(MenuInfo Menu)
        {
            var config = _IBaseConfig.GetConfig(Menu.Key);

            if (config != null && config.Value != Menu.Text)
            {
                // 更新名称
                config.Value = Menu.Text;
                _IBaseConfig.Update(config);
            }

            updateMenuProperty(Menu.Key, "href", Menu.Href);
            updateMenuProperty(Menu.Key, "icon", Menu.Icon);
            updateMenuProperty(Menu.Key, "param", Menu.Param);
            updateMenuProperty(Menu.Key, "permission", Menu.PermissionKey);
            updateMenuProperty(Menu.Key, "index", Menu.IndexValue.ToString());
        }

        public void Remove(string Key)
        {
            _IBaseConfig.Delete(Key);
        }

        private void updateMenuProperty(string key, string propertyName, string value)
        {
            var menuKey = string.Format("{0}.{1}", key, propertyName);

            var config = _IBaseConfig.GetConfig(menuKey);

            if (config != null && config.Value != value)
            {
                // 更新
                config.Value = value;
                _IBaseConfig.Update(config);
            }

            if (config == null && !string.IsNullOrEmpty(value))
            {
                // 添加
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = menuKey,
                    Value = value,
                    IsDeleted = false,
                    Type = "1"
                });
            }
        }

        private void addMenuProperty(string key, string propertyName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}", key, propertyName),
                    Value = value,
                    IsDeleted = false,
                    Type = "1"
                });
            }
        }

        private List<MenuInfo> convert(List<ConfigNode> nodes, string Key, string userIdentity, string businessKey)
        {
            var temp = new List<MenuInfo>();

            nodes.ForEach(n =>
            {
                var menu = new MenuInfo()
                {
                    Key = string.Format("{0}.Menus.{1}", Key, n.NodeName),
                    Name = n.NodeName,
                    Text = n.NodeValue
                };

                n.ChildNodes.ForEach(cn =>
                {
                    switch (cn.NodeName)
                    {
                        case "href":
                            {
                                menu.Href = cn.NodeValue;
                            }
                            break;
                        case "icon":
                            {
                                menu.Icon = cn.NodeValue;
                            }
                            break;
                        case "param":
                            {
                                menu.Param = cn.NodeValue;
                            }
                            break;
                        case "permission":
                            {
                                menu.PermissionKey = cn.NodeValue;
                            }
                            break;
                        case "index":
                            {
                                menu.IndexValue = int.Parse(cn.NodeValue);
                            }
                            break;
                        case "user":
                            {
                                if (!string.IsNullOrEmpty(userIdentity))
                                {
                                    menu.IsFavorite = isFavorite(cn, userIdentity);
                                }
                                break;
                            }
                        case "Menus":
                            {
                                menu.SubMenus = convert(cn.ChildNodes, menu.Key, userIdentity, businessKey);
                            }
                            break;
                    }
                });
                //temp.Add(menu);
                if (string.IsNullOrEmpty(menu.PermissionKey) ||
                    _IPermission.Check("data-menus-all", userIdentity, businessKey) > PermissionStatus.Reject ||    //  菜单的全部查看权
                    _IPermission.Check(menu.PermissionKey, userIdentity, businessKey) != PermissionStatus.Reject)
                {
                    temp.Add(menu);
                }
            });

            return temp.OrderBy(m => m.IndexValue).ToList();
        }

        public void Favorites(string Key, string UserIdentity)
        {
            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.user.{1}", Key, UserIdentity),
                Value = "收藏",
                IsDeleted = false,
                Type = "1",
                Tag = ""
            });
        }
        public void RemoveFavorites(string Key, string UserIdentity)
        {
            _IBaseConfig.Delete(string.Format("{0}.user.{1}", Key, UserIdentity));
        }

        private bool isFavorite(ConfigNode cn, string userID)
        {
            foreach (var node in cn.ChildNodes)
            {
                if (node.NodeName == userID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
