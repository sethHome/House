
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
    /// News Controlle
    /// </summary>
    public partial class NewsController : ApiController
    {   
		[Dependency]
		public INewsService _INewsService { get; set; }

		[Route("api/v1/news")]
        [HttpGet]
        public PageSource<NewsInfo> All(
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

            //param.FilterCondtion.Add("Kind", kind);

            return _INewsService.GetPagedList(param);
        }

		[Route("api/v1/news/{ID}")]
        [HttpGet]
        public NewsEntity One(int ID)
        {
            return this._INewsService.Get(ID);
        }

        [Token]
		[Route("api/v1/news")]
        [HttpPost]
        public int Create(NewsInfo Info)
        {
            Info.CreateUser = int.Parse(base.User.Identity.Name);
           return this._INewsService.Add(Info);
        }

		[Route("api/v1/news/{ID}")]
        [HttpPut]
        public void Update(int ID,NewsEntity Entity)
        {
            this._INewsService.Update(ID, Entity);
        }

		[Route("api/v1/news/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
			if (ID.Contains(","))
            {
                this._INewsService.Delete(ID);
            }
            else
            {
                this._INewsService.Delete(int.Parse(ID));
            }
        }

		[Route("api/v1/news")]
        [Route("api/v1/news/{ID}")]
        [HttpOptions]
        public void Option()
        { }
		
    } 
}
