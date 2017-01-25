using Api.Framework.Core;
using Api.Framework.Core.BusinessSystem;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.BusinessSystem
{
    public class BusinessSystemController : ApiController
    {
       
        [Dependency]
        public IBusinessSystem _IBusinessSystem { get; set; }

        [Route("api/v1/business")]
        [HttpGet]
        public List<BusinessSystemInfo> All(string withuser = "")
        {
            return _IBusinessSystem.All(withuser == "true");
        }

        [Route("api/v1/business/{Key}/user")]
        [HttpPut]
        public void SetUser(string Key, List<SysUserEntity> Users)
        {
            _IBusinessSystem.SetUsers(Key, Users);
        }

        [Route("api/v1/business")]
        [Route("api/v1/business/{Key}/user")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
