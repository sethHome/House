using Api.Framework.Core;
using Api.Framework.Core.Attach;
using Api.Framework.Core.File;
using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Attach
{
    public class AttachController : ApiController
    {
        [Dependency]
        public IImageGetter _IImageGetter { get; set; }

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        /// <summary>
        /// 获取附件列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("api/v1/attach")]
        [HttpGet]
        public List<SysAttachInfo> GetAttachList(string ids = "", string withtag = "false", string objkey = "", int objid = 0)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                return _IImageGetter.GetAttachs(ids, withtag.ToLower() == "true");
            }
            else if (objid > 0 && !string.IsNullOrEmpty(objkey))
            {
                var IDS = this._IObjectAttachService.GetAttachIDs(objkey, objid).ToList();

                return _IImageGetter.GetAttachs(IDS, withtag.ToLower() == "true");
            }

            return null;

        }

        [Route("api/v1/attach/{ID}")]
        [HttpGet]
        public SysAttachFileEntity GetAttachInfo(int ID)
        {
            return _IImageGetter.GetAttach(ID);
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/attach")]
        [HttpPost]
        [Token]
        public async Task<UploadResponseModel> UplodAttach()
        {
            return await RequestAttachUpload.Upload(base.Request, base.User.Identity.Name);
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/attach/open")]
        [HttpPost]
        public async Task<EditorFileInfo> UplodAttachOpen()
        {
            var file = await RequestAttachUpload.Upload(base.Request, "0");

            //return JsonConvert.SerializeObject(new EditorFileInfo()
            //{
            //    fileName = file.files[0].name,
            //    url = "http://localhost:8002/api/v1/image/" + file.files[0].id,
            //    uploaded = true,
            //    message = "success",
            //});

            var imageUrl = ConfigurationManager.AppSettings["ImageUrl"];
            return new EditorFileInfo()
            {
                fileName = file.files[0].name,
                url = imageUrl + file.files[0].id,
                uploaded = true,
                message = "success",
            };
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v1/download/{id}")]
        [HttpGet]
        public IHttpActionResult DownLoadAttach(int id)
        {
            // 进入时判断当前请求中是否含有 ETag 标识，如果有就返回使用浏览器缓存
            // Return 304

            var tag = Request.Headers.IfNoneMatch.FirstOrDefault();

            if (Request.Headers.IfModifiedSince.HasValue && tag != null && tag.Tag.Length > 0)
            {
                return new NotModifiedResponse();
            }

            return new DownloadFileActionResult(id);
        }

        /// <summary>
        /// 预览附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v1/preview/{id}")]
        [HttpGet]
        public IHttpActionResult PreviewAttach(int id)
        {
            // 进入时判断当前请求中是否含有 ETag 标识，如果有就返回使用浏览器缓存
            // Return 304

            var tag = Request.Headers.IfNoneMatch.FirstOrDefault();

            if (Request.Headers.IfModifiedSince.HasValue && tag != null && tag.Tag.Length > 0)
            {
                return new NotModifiedResponse();
            }

            return new HtmlPreviewAction(id);
        }

        /// <summary>
        /// 下载多个附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v1/download")]
        [HttpGet]
        public IHttpActionResult DownLoadAttachs(string ids = "")
        {
            // 进入时判断当前请求中是否含有 ETag 标识，如果有就返回使用浏览器缓存
            // Return 304

            var tag = Request.Headers.IfNoneMatch.FirstOrDefault();

            if (Request.Headers.IfModifiedSince.HasValue && tag != null && tag.Tag.Length > 0)
            {
                return new NotModifiedResponse();
            }

            return new DownloadFileActionResult(ids);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v1/image/{id}")]
        [HttpGet]
        public IHttpActionResult LoadImage(int id)
        {
            // 进入时判断当前请求中是否含有 ETag 标识，如果有就返回使用浏览器缓存
            // Return 304

            var tag = Request.Headers.IfNoneMatch.FirstOrDefault();

            if (Request.Headers.IfModifiedSince.HasValue && tag != null && tag.Tag.Length > 0)
            {
                return new NotModifiedResponse();
            }

            return new LoadImageActionResult(id);
        }



        [Route("api/v1/attach")]
        [Route("api/v1/attach/open")]
        [Route("api/v1/attach/{ID}")]
        [Route("api/v1/download")]
        [Route("api/v1/download/{id}")]
        [Route("api/v1/image/{id}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
