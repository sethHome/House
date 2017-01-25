using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.File;
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
    public class MTaskController : ApiController
    {
        [Dependency]
        public IMergeTaskService _IMergeTaskService { get; set; }

        [Dependency]
        public IFileService _IFileService { get; set; }

        [Route("api/system4/v1/task/{ID}/finish/{AttachID}")]
        [HttpPut]
        public void TaskFinish(int ID,int AttachID)
        {
            this._IMergeTaskService.TaskFinish(ID, AttachID);
        }

        [Token]
        [Route("api/system4/v1/task/{ID}/merge")]
        [HttpPut]
        public MergeResult Merge(int ID, MergeOption Options)
        {
            return this._IMergeTaskService.MergeDoc(ID, int.Parse(base.User.Identity.Name), Options);
        }

        [Route("api/system4/v1/task/{ID}/finish/{AttachID}")]
        [Route("api/system4/v1/task/{ID}/merge")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
