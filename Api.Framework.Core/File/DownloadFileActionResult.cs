using Api.Framework.Core.Attach;
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
    public class DownloadFileActionResult : IHttpActionResult
    {
        public DownloadFileActionResult(int fileId)
        {
            this.FileId = fileId;
        }

        public DownloadFileActionResult(string fileIDs)
        {
            this.FileIds = fileIDs;
        }

        public DownloadFileActionResult(List<SysAttachInfo> attachs)
        {
            this.Attachs = attachs;
        }

        /// <summary>
        /// 64MB
        /// </summary>
        private readonly long MEMORY_SIZE = 64 * 1024 * 1024;

        public int FileId { get; private set; }

        public string FileIds { get; private set; }

        public List<SysAttachInfo> Attachs { get; set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            if (FileId > 0)
            {
                response = await downloadSingleFile();
            }
            else 
            {
                response = downloadFiles();
            }
            return await Task.FromResult(response);
        }

        private async Task<HttpResponseMessage> downloadSingleFile()
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

                //response.Content = new StreamContent(System.IO.File.OpenRead(attach.Path));
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attach");
                response.Content.Headers.ContentDisposition.FileName = attach.Name;
                

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

        private HttpResponseMessage downloadFiles()
        {
            if (!string.IsNullOrEmpty(FileIds))
            {
                var _IImageGetter = UnityContainerHelper.GetServer<IImageGetter>();
                this.Attachs = _IImageGetter.GetAttachs(this.FileIds);
            }

            // 这里按照ID排序，以便生成的下载临时压缩文件的文件名ID有序
            this.Attachs = this.Attachs.OrderBy(a => a.ID).ToList();

            var zipContent = ZipFileHelp.ZipFileByCode(this.Attachs);
            var response = new HttpResponseMessage();
            response.Content = new StreamContent(zipContent);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = DateTime.Now.ToFileTime().ToString() + ".zip";
            
            return response;
        }
    }
}
