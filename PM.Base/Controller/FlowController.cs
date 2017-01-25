using Api.Framework.Core.BaseData;
using Api.Framework.Core.Safe;
using BPM.Engine;
using BPM.ProcessModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace PM.Base
{
    public class FlowController : ApiController
    {
        [Route("api/v1/flow/{Name}/init")]
        [HttpPost]
        public FlowNodeInfo GetInitFlowInfo(string Name,Dictionary<string,object> Params)
        {
            return FlowService.GetInitFlowNodeInfo(Name, Params);
        }

        [Route("api/v1/flow/{System}/task/{ID}")]
        [HttpPost]
        public FlowNodeInfo GetFlowNodeInfo(string System,int ID, Dictionary<string, object> Params)
        {
            return FlowService.GetFlowNodeInfo(ID, System, Params);
        }

        [Route("api/v1/flow/{System}/info")]
        [HttpGet]
        public ProcessInfo GetFlowInfo(string System, string Key = "", int ID = 0)
        {
            return FlowService.GetFlowInfo(System,Key, ID);
        }

        [Route("api/v1/flow/{System}/detail")]
        [HttpGet]
        public FlowDetailInfo GetFlowDetail(string System, string ID = "",string objkey = "",int objid = 0 )
        {
            if (!string.IsNullOrEmpty(ID))
            {
                return FlowService.GetFlowDetail(System,ID);
            }
            else
            {
                return FlowService.GetFlowDetail(System,objkey, objid);
            }
        }

        [Route("api/v1/flow/user")]
        [HttpGet]
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetFlowUser()
        {
           return FlowService.GetFlowUser();
        }

        [Route("api/v1/flow/{Name}/init")]
        [Route("api/v1/flow/{System}/info")]
        [Route("api/v1/flow/{System}/task/{ID}")]
        [Route("api/v1/flow/{System}/detail")]
        [Route("api/v1/flow/user")]
        [HttpOptions]
        public void Option()
        { }
    }
}
