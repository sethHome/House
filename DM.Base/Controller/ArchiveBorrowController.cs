using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Safe;
using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{
    public class ArchiveBorrowController : ApiController
    {
        [Dependency]
        public IBorrowService _IBorrowService { get; set; }

        [Dependency]
        public IDMUserTaskService _IDMUserTaskService { get; set; }

        [Token]
        [Route("api/v1/archiveborrow/approve")]
        [HttpGet]
        public PageSource<UserTaskInfo> GetArchiveTasks(
            int pagesize = 1000,
            int pageindex = 1,
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

            param.FilterCondtion.Add("User", int.Parse(base.User.Identity.Name));
            param.FilterCondtion.Add("Status", status);

            return _IDMUserTaskService.GetArchiveTasks(param);
        }

        [Token]
        [Route("api/v1/archiveborrow")]
        [HttpGet]
        public List<MyArchiveBorrowInfo> GetMyBorrowedArchive()
        {
            return _IBorrowService.GetMyBorrowedArchive(int.Parse(base.User.Identity.Name));
        }

        [Route("api/v1/archiveborrow/{ID}/items")]
        [HttpGet]
        public List<MyArchiveBorrowInfo> GetBorrowedArchiveItem(int ID)
        {
            return _IBorrowService.GetBorrowedArchive(ID);
        }

        [Token]
        [Route("api/v1/archiveborrow")]
        [HttpPost]
        public async Task<int> BorrowArchive(BorrowInfo Info)
        {
            Info.BorrowUser = int.Parse(base.User.Identity.Name);
            return await this._IBorrowService.BorrowArchive(Info);
        }

        [Route("api/v1/archiveborrow/{ID}/download")]
        [HttpGet]
        public IHttpActionResult Download(int ID)
        {
            return _IBorrowService.DownloadArchiveFiles(ID);
        }

        [Route("api/v1/archiveborrow/{ID}/giveback")]
        [HttpPut]
        public void GiveBack(int ID)
        {
             _IBorrowService.GiveBack(ID);
        }

        [Route("api/v1/archiveborrow/approve")]
        [Route("api/v1/archiveborrow/{ID}/download")]
        [Route("api/v1/archiveborrow/{ID}/giveback")]
        [Route("api/v1/archiveborrow/{ID}/items")]
        [Route("api/v1/archiveborrow")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
