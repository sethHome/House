using Api.Framework.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Settings
{
    public interface ISettingsService
    {
        Dictionary<string, string> GetSettingDic();

        void UpdateSettings(Dictionary<string, string> Settings);

        List<ConfigNode> GetSettings();

        string GetSettingValue(string Key);
    }
}
