
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Tag;

namespace Api.System.Attach
{   
    /// <summary>
    /// SysTag Controlle
    /// </summary>
    public partial class SysTagController : ApiController
    {   
		[Dependency]
		public ISysTagService _ISysTagService { get; set; }

		[Route("api/v1/tag")]
        [HttpGet]
        public IEnumerable<SysTagEntity> All(
			int pagesize = 1000,
            int pageindex = 1,
            string orderby = "",
			string txtfilter = "",
            string objectkey = "")
        {
			var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
                TextCondtion = txtfilter,
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("ObjectKey", objectkey);

            var result = _ISysTagService.GetPagedList(param);

            return result.Source;
        }

		[Route("api/v1/systag/{ID}")]
        [HttpGet]
        public SysTagEntity One(int ID)
        {
            return this._ISysTagService.Get(ID);
        }

		[Route("api/v1/systag")]
        [HttpPost]
        public int Create(SysTagEntity Entity)
        {
            this._ISysTagService.Add(Entity);

            return Entity.ID;
        }

		[Route("api/v1/systag/{ID}")]
        [HttpPut]
        public void Update(int ID,SysTagEntity Entity)
        {
            this._ISysTagService.Update(ID, Entity);
        }

		[Route("api/v1/systag/{ID}")]
        [HttpDelete]
        public void Delete(int ID)
        {
            this._ISysTagService.Delete(ID);
        }

        [Route("api/v1/tag")]
        [Route("api/v1/systag")]
        [Route("api/v1/systag/{ID}")]
        [HttpOptions]
        public void Options()
        {
        }
    } 
}
