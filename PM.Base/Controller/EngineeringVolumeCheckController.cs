
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
    /// EngineeringVolumeCheck Controlle
    /// </summary>
    public partial class EngineeringVolumeCheckController : ApiController
    {
        [Dependency]
        public IEngineeringVolumeCheckService _IEngineeringVolumeCheckService { get; set; }

        [Route("api/v1/volume/{ID}/check")]
        [HttpGet]
        public PageSource<EngineeringVolumeCheckEntity> All(
            int ID,
            int pagesize = 1000,
            int pageindex = 1,
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

            param.FilterCondtion.Add("VolumeID", ID);

            return _IEngineeringVolumeCheckService.GetPagedList(param);
        }


        [Token]
        [Route("api/v1/volume/{ID}/check")]
        [HttpPost]
        public EngineeringVolumeCheckEntity BatchUpdate(int ID, EngineeringVolumeCheckEntity Entity)
        {
            Entity.VolumeID = ID;
            Entity.UserID = int.Parse(base.User.Identity.Name);
            return this._IEngineeringVolumeCheckService.Add(Entity);
        }

        [Route("api/v1/volume/check/{ID}")]
        [HttpPut]
        public void Set(int ID, EngineeringVolumeCheckEntity Entity)
        {
            this._IEngineeringVolumeCheckService.Update(ID, Entity);
        }

        [Route("api/v1/volume/check/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringVolumeCheckService.Delete(ID);
            }
            else
            {
                this._IEngineeringVolumeCheckService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/volume/check/{ID}")]
        [Route("api/v1/volume/{ID}/check")]
        [HttpOptions]
        public void Option()
        { }

    }
}
