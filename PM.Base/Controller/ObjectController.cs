
using Api.Framework.Core;
using Api.Framework.Core.Safe;
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
    public class ObjectController : ApiController
    {
        [Dependency("Project")]
        public ITopBizObject _IProjTopBizObject { get; set; }

        [Dependency("Engineering")]
        public ITopBizObject _IEngTopBizObject { get; set; }

        [Token]
        [Route("api/v1/object/tree")]
        [HttpGet]
        public List<BizObject> GetTree(int EngineeringType = 0,int EngineeringPhase = 0,int VolLev = 0,int DateRange = 0,int deep = int.MaxValue,int task = 0,
            string text = "")
        {
            var param = new PageQueryParam();
            param.FilterCondtion = new System.Collections.Hashtable();
            param.FilterCondtion.Add("Engineering.Text", text);
            param.FilterCondtion.Add("Engineering.Type", EngineeringType);
            param.FilterCondtion.Add("Engineering.Phase", EngineeringPhase);
            param.FilterCondtion.Add("Engineering.VolLevel", VolLev);
            param.FilterCondtion.Add("Engineering.DateRange", DateRange);
            param.FilterCondtion.Add("CurrentUser", int.Parse(base.User.Identity.Name));
            if (task > 0)
            {
                param.FilterCondtion.Add("Task", 1);
            }
            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IProjTopBizObject.Get(param, deep);
        }

        [Token]
        [Route("api/v1/object/engineering/tree")]
        [HttpGet]
        public List<BizObject> GetEngineeringTree(int projectid = 0, int engid=0, int EngineeringType = 0, int EngineeringPhase = 0, int VolLev = 0, int DateRange = 0, int deep = int.MaxValue, int task = 0,
            string text = "")
        {
            var param = new PageQueryParam();
            param.FilterCondtion = new System.Collections.Hashtable();
            param.FilterCondtion.Add("Project.ID", projectid);
            param.FilterCondtion.Add("Engineering.ID", engid);
            param.FilterCondtion.Add("Engineering.Text", text);
            param.FilterCondtion.Add("Engineering.Type", EngineeringType);
            param.FilterCondtion.Add("Engineering.Phase", EngineeringPhase);
            param.FilterCondtion.Add("Engineering.VolLevel", VolLev);
            param.FilterCondtion.Add("Engineering.DateRange", DateRange);
            param.FilterCondtion.Add("CurrentUser", int.Parse(base.User.Identity.Name));
            if (task > 0)
            {
                param.FilterCondtion.Add("Task", 1);
            }
            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IEngTopBizObject.Get(param, deep);
        }

        [Route("api/v1/object/tree")]
        [Route("api/v1/object/engineering/tree")]
        [HttpOptions]
        public void Option()
        { }
    }
}
