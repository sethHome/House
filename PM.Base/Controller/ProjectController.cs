using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using System.Collections;

namespace PM.Base
{   
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public partial class ProjectController : ApiController
    {   
		[Dependency]
		public IProjectService _IProjectService { get; set; }

        [Route("api/v1/project")]
        [HttpGet]
        public PageSource<ProjectInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            bool trash = false,
            int kind = 0, int type = 0, int vollevel = 0, int manager = 0, int secretlevel = 0,
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
                IsDelete = trash,
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("Kind", kind);
            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Vollevel", vollevel);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("Secretlevel", secretlevel);

            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IProjectService.GetPagedList(param);
        }

        [Route("api/v1/project/source")]
        [HttpGet]
        public List<ProjectEntity> Source(string number = "", string name = "")
        {
            return this._IProjectService.GetSource(number, name);
        }

        [Route("api/v1/project/{ID}")]
        [HttpGet]
        public ProjectEntity One(int ID)
        {
            return this._IProjectService.Get(ID);
        }

		[Route("api/v1/project/{ID}")]
        [HttpPut]
        public void Update(int ID,ProjectEntity Entity)
        {
            this._IProjectService.Update(ID, Entity);
        }

        [Route("api/v1/project/{IDs}/backup")]
        [HttpDelete]
        public void BackUp(string IDs)
        {
            this._IProjectService.BackUp(IDs);
        }

        [Route("api/v1/project")]
        [HttpPost]
        public int Create(ProjectInfo Entity)
        {
            return this._IProjectService.Add(Entity);
        }

        [Route("api/v1/project/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IProjectService.Delete(ID);
            }
            else
            {
                this._IProjectService.Delete(int.Parse(ID));
            }
            
        }

        [Route("api/v1/project")]
        [Route("api/v1/project/source")]
        [Route("api/v1/project/{ID}")]
        [Route("api/v1/project/{IDs}/backup")]
        [HttpOptions]
        public void Options()
        {
        }
    } 
}
