using DM.Base.Entity;
using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{
    public class ArchiveLogController : ApiController
    {
        [Dependency]
        public IArchiveLogService _IArchiveLogService { get; set; }

        [Route("api/v1/archivelog")]
        [HttpGet]
        public List<ArchiveLogEntity> GetArchiveLog(string fonds = "",string type= "", int id = 0)
        {
            return _IArchiveLogService.GetArchiveLogs(fonds,type,id);
        }

        [Route("api/v1/archivelog")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
