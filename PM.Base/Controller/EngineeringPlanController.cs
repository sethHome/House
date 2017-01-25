
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringPlan Controlle
    /// </summary>
    public partial class EngineeringPlanController : ApiController
    {   
		[Dependency]
		public IEngineeringPlanService _IEngineeringPlanService { get; set; }

		[Route("api/v1/engineeringplan")]
        [HttpGet]
        public PageSource<EngineeringPlanInfo> All(
			int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
			string orderdirection = "desc",
			string txtfilter = "")
        {
			var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
				IsDesc = orderdirection.ToLower().Equals("desc"),
                TextCondtion = txtfilter,
                FilterCondtion = new Hashtable(),
            };

            //param.FilterCondtion.Add("Kind", kind);

            return _IEngineeringPlanService.GetPagedList(param);
        }

		[Route("api/v1/engineeringplan/{ID}")]
        [HttpGet]
        public EngineeringPlanEntity One(int ID)
        {
            return this._IEngineeringPlanService.Get(ID);
        }

		[Route("api/v1/engineeringplan")]
        [HttpPost]
        public int Create(EngineeringPlanInfo Info)
        {
           return this._IEngineeringPlanService.Add(Info);
        }

		[Route("api/v1/engineeringplan/{ID}")]
        [HttpPut]
        public void Update(int ID,EngineeringPlanEntity Entity)
        {
            this._IEngineeringPlanService.Update(ID, Entity);
        }

		[Route("api/v1/engineeringplan/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
			if (ID.Contains(","))
            {
                this._IEngineeringPlanService.Delete(ID);
            }
            else
            {
                this._IEngineeringPlanService.Delete(int.Parse(ID));
            }
        }

		[Route("api/v1/engineeringplan")]
        [Route("api/v1/engineeringplan/{ID}")]
        [HttpOptions]
        public void Option()
        { }
		
    } 
}
