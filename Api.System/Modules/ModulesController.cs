using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Modules
{

    public class ModulesController : ApiController
    {
        [Dependency]
        public IModule _IModule { get; set; }

        [Token]
        [Route("api/v1/module")]
        [HttpGet]
        public List<ModuleInfo> GetModule(string business = "")
        {
            return _IModule.GetModules(base.User.Identity.Name, business);
        }

        [Route("api/v1/module")]
        [HttpPost]
        public void AddModule(CreateModuleInfo Module)
        {
             _IModule.AddModule(Module);
        }

        [Route("api/v1/module")]
        [HttpDelete]
        public void RemoveModule(string Key = "")
        {
            _IModule.RemoveModule(Key);
        }

        [Route("api/v1/module")]
        [HttpPut]
        public void RemoveModule(ModuleInfo Module)
        {
            _IModule.UpdateModule(Module);
        }

        [Route("api/v1/module")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
