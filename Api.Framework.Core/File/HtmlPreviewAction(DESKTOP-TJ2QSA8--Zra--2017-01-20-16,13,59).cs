using Api.Framework.Core.Office;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class HtmlPreviewAction : IHttpActionResult
    {
        public HtmlPreviewAction(int fileId)
        {
            this.FileId = fileId;
        }


        / <summary>
        / 64MB
        / </summary>
        private readonly long MEMORY_SIZE = 64 * 1024 * 1024;

        public int FileId { get; private set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            //return await Task.FromResult(getResponse1());
            return getResponse2();
        }

        private HttpResponseMessage getResponse1()
        {
            var _IImageGetter = UnityContainerHelper.GetServer<IImageGetter>();

            var attach = _IImageGetter.GetAttach(this.FileId);

            var response = new HttpResponseMessage();

            if (attach != null && System.IO.File.Exists(attach.Path))
            {
                var stream = WordGenerator.ConvertHtml(attach.Path);

                // 把图片的绝对路径替换成网络路径
                var htmlStr = Encoding.UTF8.GetString(stream.ToArray());
                htmlStr = htmlStr.Replace(AppDomain.CurrentDomain.BaseDirectory, "http://localhost:8002/");
                byte[] bytes = Encoding.UTF8.GetBytes(htmlStr);

                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

                // 设置缓存信息，该部分可以没有，该部分主要是用于与开始部分结合以便浏览器使用304缓存
                // Set Cache
                response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now).AddHours(1);
                // 这里应该写入文件的存储日期
                response.Content.Headers.LastModified = new DateTimeOffset(DateTime.Now);
                response.Headers.CacheControl = new CacheControlHeaderValue() { Public = true, MaxAge = TimeSpan.FromHours(1) };
                // 设置Etag，这里就简单采用 Id
                response.Headers.ETag = new EntityTagHeaderValue(string.Format("\"{0}\"", FileId));

                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }


        private async Task<HttpResponseMessage> getResponse2()
        {
            var _IImageGetter = UnityContainerHelper.GetServer<IImageGetter>();

            var attach = _IImageGetter.GetAttach(this.FileId);

            var response = new HttpResponseMessage();

            if (attach != null && System.IO.File.Exists(attach.Path))
            {
                var file = new FileStream(attach.Path, FileMode.Open, FileAccess.Read, FileShare.Read);

                // 判断是否大于64Md，如果大于就采用分段流返回，否则直接返回
                if (file.Length < MEMORY_SIZE)
                {
                    //Copy To Memory And Close.
                    byte[] bytes = new byte[file.Length];
                    await file.ReadAsync(bytes, 0, (int)file.Length);
                    file.Close();
                    MemoryStream ms = new MemoryStream(bytes);

                    response.Content = new ByteArrayContent(ms.ToArray());
                }
                else
                {
                    response.Content = new StreamContent(file);
                }

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                // 设置缓存信息，该部分可以没有，该部分主要是用于与开始部分结合以便浏览器使用304缓存
                // Set Cache
                response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now).AddHours(1);
                // 这里应该写入文件的存储日期
                response.Content.Headers.LastModified = new DateTimeOffset(DateTime.Now);
                response.Headers.CacheControl = new CacheControlHeaderValue() { Public = true, MaxAge = TimeSpan.FromHours(1) };
                // 设置Etag，这里就简单采用 Id
                response.Headers.ETag = new EntityTagHeaderValue(string.Format("\"{0}\"", FileId));

                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }

    }
}
