
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using BPM.DB;

namespace PM.Base
{
    /// <summary>
    /// UserTask Controlle
    /// </summary>
    public partial class UserTaskController : ApiController
    {
        [Dependency]
        public IPMUserTaskService _IPMUserTaskService { get; set; }

        [Dependency]
        public IEngineeringVolumeService _IEngineeringVolumeService { get; set; }

        [Token]
        [Route("api/v1/task/count")]
        [HttpGet]
        public Dictionary<int, int> GetUserTaskCount()
        {
            return _IPMUserTaskService.GetUserTaskCount(int.Parse(base.User.Identity.Name));
        }

        [Token]
        [Route("api/v1/task/production")]
        [HttpGet]
        public PageSource<UserTaskInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            int user = 0,
            int status = 0,
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

            param.FilterCondtion.Add("User", user);
            param.FilterCondtion.Add("Status", status);

            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IPMUserTaskService.GetProductionTasks(param);
        }

        [Route("api/v1/task/provide")]
        [HttpGet]
        public PageSource<UserTaskInfo> AllProvide(
            int pagesize = 1000,
            int pageindex = 1,
            int user = 0,
            int status = 0,
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

            param.FilterCondtion.Add("User", user);
            param.FilterCondtion.Add("Status", status);

            return _IPMUserTaskService.GetProvideTasks(param);
        }

        [Route("api/v1/task/form")]
        [HttpGet]
        public PageSource<UserTaskInfo> AllForm(
            int pagesize = 1000,
            int pageindex = 1,
            int user = 0,
            int status = 0,
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

            param.FilterCondtion.Add("User", user);
            param.FilterCondtion.Add("Status", status);

            return _IPMUserTaskService.GetFormTasks(param);
        }

        [Route("api/v1/task/{ID}")]
        [HttpGet]
        public UserTaskEntity One(int ID)
        {
            return this._IPMUserTaskService.Get(ID);
        }

        [Route("api/v1/task/{ID}/volume")]
        [HttpGet]
        public EngineeringVolumeInfo GetTaskVolumeInfo(int ID)
        {
            return this._IEngineeringVolumeService.GetVolumeInfo(ID);
        }

        [Route("api/v1/task")]
        [HttpPost]
        public int Create(UserTaskEntity Info)
        {
            return this._IPMUserTaskService.Add(Info);
        }

        [Route("api/v1/task/{ID}")]
        [HttpPut]
        public void Update(int ID, UserTaskEntity Entity)
        {
            this._IPMUserTaskService.Update(ID, Entity);
        }

        [Route("api/v1/task/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IPMUserTaskService.Delete(ID);
            }
            else
            {
                this._IPMUserTaskService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/task")]
        [Route("api/v1/task/count")]
        [Route("api/v1/task/production")]
        [Route("api/v1/task/provide")]
        [Route("api/v1/task/form")]
        [Route("api/v1/task/{ID}")]
        [Route("api/v1/task/{ID}/volume")]
        [HttpOptions]
        public void Option()
        { }

    }
}
