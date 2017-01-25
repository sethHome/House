using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using System.Linq;

namespace Api.System.Attach
{
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class ObjectAttachController : ApiController
    {
        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }


        [Route("api/v1/object/{ObjectKey}/{ObjectID}/attach")]
        [HttpGet]
        public List<int> GetObjectAttachIDs(string ObjectKey,int ObjectID)
        {
            return this._IObjectAttachService.GetAttachIDs(ObjectKey, ObjectID).ToList();

        }

        [Route("api/v1/attach/{ID}/object")]
        [HttpPost]
        public void SetObjectAttach(int ID, ObjectAttachEntity Entity)
        {
            Entity.AttachID = ID;
            this._IObjectAttachService.Add(Entity);
        }

        [Route("api/v1/attach/{ID}/object/{ObjectName}")]
        [HttpDelete]
        public void Delete(int ID, string ObjectName)
        {
            this._IObjectAttachService.Delete(ID, ObjectName);
        }

        [Route("api/v1/attach/object/{ObjectName}")]
        [HttpDelete]
        public void Delete(string ObjectName, string ids = "")
        {
            if (ids.Length > 0) {
                var atatchIDs = ids.Split(',');
                foreach (var id in atatchIDs)
                {
                    this._IObjectAttachService.Delete(int.Parse(id), ObjectName);
                }
            }
        }

        [Route("api/v1/object/{ObjectKey}/{ObjectID}/attach")]
        [Route("api/v1/attach/{ID}/object")]
        [Route("api/v1/attach/{ID}/object/{ObjectName}")]
        [Route("api/v1/attach/object/{ObjectName}")]
        [HttpOptions]
        public void Options()
        {
        }

    }
}
