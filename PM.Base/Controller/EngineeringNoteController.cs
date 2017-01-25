
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
    /// EngineeringNote Controlle
    /// </summary>
    public partial class EngineeringNoteController : ApiController
    {
        [Dependency]
        public IEngineeringNoteService _IEngineeringNoteService { get; set; }

        [Route("api/v1/note")]
        [HttpGet]
        public PageSource<EngineeringNoteInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            string ids = "",
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

            param.FilterCondtion.Add("IDs", ids);
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

            return _IEngineeringNoteService.GetPagedList(param);
        }

        [Route("api/v1/note/{ID}")]
        [HttpGet]
        public EngineeringNoteInfo One(int ID)
        {
            return this._IEngineeringNoteService.Get(ID);
        }

        [Token]
        [Route("api/v1/note")]
        [HttpPost]
        public int Create(EngineeringNoteInfo Info)
        {
            Info.UserID = int.Parse(base.User.Identity.Name);
            return this._IEngineeringNoteService.Add(Info);
        }

        [Route("api/v1/note/{ID}")]
        [HttpPut]
        public void Update(int ID, EngineeringNoteEntity Entity)
        {
            this._IEngineeringNoteService.Update(ID, Entity);
        }

        [Route("api/v1/note/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringNoteService.Delete(ID);
            }
            else
            {
                this._IEngineeringNoteService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/note")]
        [Route("api/v1/note/{ID}")]
        [HttpOptions]
        public void Option()
        { }

    }
}
