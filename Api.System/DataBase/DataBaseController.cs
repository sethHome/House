using Api.Framework.Core;
using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.File;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
namespace Api.System.DataBase
{
    public class DataBaseController : ApiController
    {
        [Dependency]
        public IDataBaseService _IDataBaseService { get; set; }


        [Route("api/v1/database/{Name}/back")]
        [HttpGet]
        public IHttpActionResult Backup(string Name)
        {
            var fileID = _IDataBaseService.BackUp(Name);

            return new DownloadFileActionResult(fileID);
        }

        [Route("api/v1/database/{Name}/back")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
