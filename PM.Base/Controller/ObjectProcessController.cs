using Api.Framework.Core.Safe;
using BPM.DB;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PM.Base.Controller
{
    public class ObjectProcessController : ApiController
    {
        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

       
        [Route("api/v1/object/process")]
        [HttpPost]
        public void Create(ObjectProcessEntity Info)
        {
            _IObjectProcessService.Add(Info);
        }

        [Route("api/v1/object/process")]
        [HttpOptions]
        public void Option()
        { }
    }
}
