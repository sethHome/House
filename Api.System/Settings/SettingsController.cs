using Api.Framework.Core;
using Api.Framework.Core.Config;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Settings
{
    public class SettingsController : ApiController
    {
        [Dependency]
        public ISettingsService _ISettingsService { get; set; }

        [Route("api/v1/settings")]
        [HttpGet]
        public List<ConfigNode> GetSettings()
        {
            return _ISettingsService.GetSettings();
        }

        [Route("api/v1/settings/dic")]
        [HttpGet]
        public Dictionary<string, string> GetSettingsDic()
        {
            return _ISettingsService.GetSettingDic();
        }

        [Route("api/v1/settings")]
        [HttpPut]
        public void UpdateSettings(Dictionary<string,string> items)
        {
             _ISettingsService.UpdateSettings(items);
        }

        [Route("api/v1/settings")]
        [Route("api/v1/settings/dic")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
