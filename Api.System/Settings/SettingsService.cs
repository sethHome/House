using Api.Framework.Core.Attach;
using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Settings
{
    public class SettingsService : ISettingsService
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public Dictionary<string, string> GetSettingDic()
        {
            
            var configs = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith("Settings."), "Settings.", false);
            var result = new Dictionary<string, string>();

            foreach (var config in configs)
            {
                foreach (var node in config.ChildNodes)
                {
                    if (node.Propertys["DataType"] == "File")
                    {
                        var idList = _IObjectAttachService.GetAttachIDs(node.NodeName, int.Parse(node.Propertys["Value"]));
                        string ids = string.Join(",", idList.ToArray());
                        result.Add(string.Format("{0}.{1}", config.NodeName, node.NodeName), ids);
                    }
                    else
                    {
                        result.Add(string.Format("{0}.{1}", config.NodeName, node.NodeName), node.Propertys["Value"]);
                    }
                }
            }

            return result;
        }

        public List<ConfigNode> GetSettings()
        {
            return _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith("Settings."), "Settings.", false);
        }

        public string GetSettingValue(string Key)
        {
            var key = string.Format("Settings.{0}.Value", Key);

            var config = _IBaseConfig.GetConfig(key);

            return config.Value;
        }

        public void UpdateSettings(Dictionary<string, string> Settings)
        {
            foreach (var item in Settings)
            {
                var key = string.Format("Settings.{0}.Value", item.Key);
                var config = _IBaseConfig.GetConfig(key);
                config.Value = item.Value;

                _IBaseConfig.Update(config);
            }
        }
    }
}
