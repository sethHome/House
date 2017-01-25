using Api.Framework.Core;
using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Modules
{
    public class ModuleService : IModule
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IBusinessSystem _IBusinessSystem { get; set; }

        /// <summary>
        /// 模块配置Key中保留的关键字
        /// </summary>
        private string[] _DisableKey = new string[] { "src", "Modules" };

        public List<ModuleInfo> GetModules(string UserID,string SysKey = "")
        {
            var result = new List<ModuleInfo>();

            var sysKeys = _IBusinessSystem.GetBusiness(int.Parse(UserID)).Select(c => c.Key);

            //var sysKeys = _IBusinessSystem.All().Select(c => c.Key);

            foreach (var key in sysKeys)
            {
                if (SysKey == "" || key == SysKey)
                {
                    var part = string.Format("BusinessSystem.{0}.Modules.", key);

                    var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

                    convertToModule(nodes, result, string.Format("BusinessSystem.{0}", key), key);
                }
            }

            return result;
        }

        public void AddModule(CreateModuleInfo Module)
        {
            var moduleKey = "";

            if (!string.IsNullOrEmpty(Module.ParentKey))
            {
                moduleKey = string.Format("{0}.Modules.{1}", Module.ParentKey, Module.Name);
            }
            else if (!string.IsNullOrEmpty(Module.BusinessKey))
            {
                moduleKey = string.Format("BusinessSystem.{0}.Modules.{1}", Module.BusinessKey, Module.Name);
            }
            else
            {
                throw new Exception("必须指定业务系统或者父级模块的Key");
            }

            if (_DisableKey.Contains(Module.Name))
            {
                throw new Exception(string.Format("使用了模块保留关键字：{0}", Module.Name));
            }

            if (string.IsNullOrEmpty(Module.Name))
            {
                throw new Exception("模块Key不能为空");
            }

            var module = new ConfigEntity()
            {
                Key = moduleKey,
                Value = Module.Text,
                IsDeleted = false,
                Type = "1"
            };
            _IBaseConfig.Add(module);

            addProperty(moduleKey, "src", Module.Src);
            addProperty(moduleKey, "param", Module.Param);
            addProperty(moduleKey, "tab", Module.Tab.ToString());
        }

        public void RemoveModule(string Key)
        {
            _IBaseConfig.Delete(Key);
        }

        public void UpdateModule(ModuleInfo Module)
        {
            var config = _IBaseConfig.GetConfig(Module.Key);

            if (config != null && config.Value != Module.Text)
            {
                // 更新模块名称
                config.Value = Module.Text;
                _IBaseConfig.Update(config);
            }

            updateProperty(Module.Key, "src", Module.Src);
            updateProperty(Module.Key, "param", Module.Param);
            updateProperty(Module.Key, "tab", Module.Tab.ToString());
        }

        private void convertToModule(List<ConfigNode> nodes, List<ModuleInfo> modules, string Key,string system)
        {
            nodes.ForEach(n =>
            {
                var module = new ModuleInfo()
                {
                    Key = string.Format("{0}.Modules.{1}", Key, n.NodeName),
                    Name = n.NodeName,
                    Text = n.NodeValue,
                    System = system
                };

                n.ChildNodes.ForEach(cn =>
                {
                    switch (cn.NodeName)
                    {
                        case "src":
                            {
                                module.Src = cn.NodeValue;
                            }
                            break;
                        case "param":
                            {
                                module.Param = cn.NodeValue;
                            }
                            break;
                        case "tab":
                            {
                                module.Tab = Convert.ToBoolean(cn.NodeValue);
                            }
                            break;
                        case "Modules":
                            {
                                module.SubModules = new List<ModuleInfo>();
                                convertToModule(cn.ChildNodes, module.SubModules, module.Key, system);
                            }
                            break;
                    }
                });

                modules.Add(module);
            });
        }

        private void updateProperty(string key, string propertyName, string value)
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

        private void addProperty(string key, string propertyName, string value)
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
    }
}
