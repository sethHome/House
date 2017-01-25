using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Safe;
using Merge.Base.Entitys;
using Merge.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Merge.Base.Controller
{
    public class MProjectController : ApiController
    {
        [Dependency]
        public IMProjectService _IProjectService { get; set; }

        [Token]
        [Route("api/system4/v1/project")]
        [HttpGet]
        public PageSource<ProjectInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            int myproj = 0,
            int area = 0, string number = "", int manager = 0,
            string createdatefrom = "", string createdateto = "")
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

            param.FilterCondtion.Add("Area", area);
            param.FilterCondtion.Add("Number", number);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            if (myproj > 0)
            {
                param.FilterCondtion.Add("Manager", int.Parse(base.User.Identity.Name));
            }
            else
            {
                param.FilterCondtion.Add("Manager", manager);
            }
            return _IProjectService.GetPagedList(param);
        }

        [Token]
        [Route("api/system4/v1/project/mytask")]
        [HttpGet]
        public List<ProjectInfo> GetMyTask()
        {
            return this._IProjectService.GetMyTask(int.Parse(base.User.Identity.Name));
        }

        [Route("api/system4/v1/project/{ID}")]
        [HttpGet]
        public ProjectEntity One(int ID)
        {
            return this._IProjectService.Get(ID);
        }

        [Route("api/system4/v1/project/{ID}")]
        [HttpPut]
        public void Update(int ID, ProjectInfo Entity)
        {
            this._IProjectService.Update(ID, Entity);
        }

        [Route("api/system4/v1/project")]
        [HttpPost]
        public int Create(ProjectInfo Entity)
        {
            return this._IProjectService.Add(Entity);
        }

        [Route("api/system4/v1/project/{ID}")]
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

        [Route("api/system4/v1/project")]
        [Route("api/system4/v1/project/mytask")]
        [Route("api/system4/v1/project/{ID}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
