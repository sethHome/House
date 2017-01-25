
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
    /// ObjectTag Controlle
    /// </summary>
    public partial class ObjectTagController : ApiController
    {   
		[Dependency]
		public IObjectTagService _IObjectTagService { get; set; }

		//[Route("api/v1/objecttag")]
  //      [HttpGet]
  //      public PageSource<ObjectTagEntity> All(
		//	int pagesize = 1000,
  //          int pageindex = 1,
  //          string orderby = "",
		//	string txtfilter = "")
  //      {
		//	var param = new PageQueryParam()
  //          {
  //              PageSize = pagesize,
  //              PageIndex = pageindex,
  //              IsAllowPage = pagesize > 0 && pageindex > 0,
  //              OrderFiled = orderby,
  //              TextCondtion = txtfilter,
  //              FilterCondtion = new Hashtable(),
  //          };

  //          //param.FilterCondtion.Add("Kind", kind);

  //          return _IObjectTagService.GetPagedList(param);
  //      }

		//[Route("api/v1/objecttag/{ID}")]
  //      [HttpGet]
  //      public ObjectTagEntity One(int ID)
  //      {
  //          return this._IObjectTagService.Get(ID);
  //      }

        /// <summary>
        /// 业务对象添加标签
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
		[Route("api/v1/object/{Key}/{ID}/tag")]
        [HttpPost]
        public void Create(string Key,int ID ,List<ObjectTagInfo> Tags)
        {
            this._IObjectTagService.Add(Key, ID,Tags);
        }

        //[Route("api/v1/objecttag/{ID}")]
        //      [HttpPut]
        //      public void Update(int ID,ObjectTagEntity Entity)
        //      {
        //          this._IObjectTagService.Update(ID, Entity);
        //      }

        //[Route("api/v1/objecttag/{ID}")]
        //      [HttpDelete]
        //      public void Delete(int ID)
        //      {
        //          this._IObjectTagService.Delete(ID);
        //      }

        [Route("api/v1/object/{Key}/{ID}/tag")]
        [HttpOptions]
        public void Options()
        {
        }
    } 
}
