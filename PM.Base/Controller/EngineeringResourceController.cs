
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
    /// EngineeringResource Controlle
    /// </summary>
    public partial class EngineeringResourceController : ApiController
    {
        [Dependency]
        public IEngineeringResourceService _IEngineeringResourceService { get; set; }

        [Route("api/v1/resource")]
        [HttpGet]
        public PageSource<EngineeringResourceInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            int type = 0, int phase = 0, int vollevel = 0, int tasktype = 0, int status = 0, int manager = 0,
            string createdatefrom = "", string createdateto = "",
            string deliverydatefrom = "", string deliverydateto = "")
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

            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Phase", phase);
            param.FilterCondtion.Add("VolLevel", vollevel);
            param.FilterCondtion.Add("TaskType", tasktype);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IEngineeringResourceService.GetPagedList(param);
        }

        [Route("api/v1/resource/{ID}")]
        [HttpGet]
        public EngineeringResourceEntity One(int ID)
        {
            return this._IEngineeringResourceService.Get(ID);
        }

        [Token]
        [Route("api/v1/resource")]
        [HttpPost]
        public int Create(EngineeringResourceInfo Info)
        {
            Info.UserID = int.Parse(base.User.Identity.Name);
            return this._IEngineeringResourceService.Add(Info);
        }

        [Route("api/v1/resource/{ID}")]
        [HttpPut]
        public void Update(int ID, EngineeringResourceEntity Entity)
        {
            this._IEngineeringResourceService.Update(ID, Entity);
        }

        [Route("api/v1/resource/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringResourceService.Delete(ID);
            }
            else
            {
                this._IEngineeringResourceService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/resource")]
        [Route("api/v1/resource/{ID}")]
        [HttpOptions]
        public void Option()
        { }

    }
}
