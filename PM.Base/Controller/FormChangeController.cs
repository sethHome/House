
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;
using Api.Framework.Core.File;

namespace PM.Base
{
    /// <summary>
    /// FormChange Controlle
    /// </summary>
    public partial class FormController : ApiController
    {
        [Dependency]
        public IFormChangeService _IFormChangeService { get; set; }


        [Route("api/v1/form/change")]
        [HttpGet]
        public PageSource<FormChangeInfo> All(
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

            return _IFormChangeService.GetPagedList(param);
        }

        [Route("api/v1/form/change/{ID}")]
        [HttpGet]
        public FormChangeInfo One(int ID)
        {
            return this._IFormChangeService.Get(ID);
        }

        [Route("api/v1/form/change/{ID}/download")]
        [HttpGet]
        public IHttpActionResult Download(int ID)
        {
            // 进入时判断当前请求中是否含有 ETag 标识，如果有就返回使用浏览器缓存
            // Return 304

            var tag = Request.Headers.IfNoneMatch.FirstOrDefault();

            if (Request.Headers.IfModifiedSince.HasValue && tag != null && tag.Tag.Length > 0)
            {
                return new NotModifiedResponse();
            }

            return this._IFormChangeService.Export(ID);
        }

        [Token]
        [Route("api/v1/form/change")]
        [HttpPost]
        public async Task<int> Create(FormChangeInfo Info)
        {
            Info.CreateUserID = int.Parse(base.User.Identity.Name);
            return await this._IFormChangeService.Add(Info);
        }

        [Route("api/v1/form/change/{ID}")]
        [HttpPut]
        public void Update(int ID, FormChangeEntity Entity)
        {
            this._IFormChangeService.Update(ID, Entity);
        }

        [Route("api/v1/form/change/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IFormChangeService.Delete(ID);
            }
            else
            {
                this._IFormChangeService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/form/change")]
        [Route("api/v1/form/change/{ID}")]
        [HttpOptions]
        public void Option()
        { }
    }
}
