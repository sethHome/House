using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Framework.Core.File
{
    public class LoadImageActionResult : IHttpActionResult
    {
        public LoadImageActionResult(int imageID)
        {
            this.ImageID = imageID;
        }

        public int ImageID { get; private set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var _IImageGetter = UnityContainerHelper.GetServer<IImageGetter>();

            //if (System.IO.File.Exists(filePath))
            //{
            //    HttpResponseMessage response = new HttpResponseMessage();
            //    response.Content = new StreamContent(System.IO.File.OpenRead(filePath));
            //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            //    return await Task.FromResult(response);
            //}

            var attach = _IImageGetter.GetAttach(this.ImageID);

            HttpResponseMessage response = new HttpResponseMessage();

            if (attach != null && attach.Type == (int)EnumAttachType.Picture && System.IO.File.Exists(attach.Path))
            {
                response.Content = new StreamContent(System.IO.File.OpenRead(attach.Path));
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + attach.Extension.Trim('.'));
                
                // 设置缓存信息，该部分可以没有，该部分主要是用于与开始部分结合以便浏览器使用304缓存
                // Set Cache
                response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now).AddHours(1);
                // 这里应该写入文件的存储日期
                response.Content.Headers.LastModified = new DateTimeOffset(DateTime.Now);
                response.Headers.CacheControl = new CacheControlHeaderValue() { Public = true, MaxAge = TimeSpan.FromHours(1) };
                // 设置Etag，这里就简单采用 Id
                response.Headers.ETag = new EntityTagHeaderValue(string.Format("\"{0}\"", ImageID));

                response.StatusCode = HttpStatusCode.OK;
            }

            return await Task.FromResult(response);
        }
    }
}
