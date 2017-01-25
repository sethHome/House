using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace DM.Base.Controller
{
    public class ArgumentSettingController : ApiController
    {
        [Dependency]
        public IArgumentSettingService _IArgumentSettingService { get; set; }

        [Route("api/v1/argumentsetting/mapping")]
        [HttpPost]
        public void Mapping(List<FieldMapping> Mappings)
        {
            _IArgumentSettingService.FieldMapping(Mappings);
        }

        [Route("api/v1/argumentsetting/mapping")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
