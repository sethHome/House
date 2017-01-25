using Api.Framework.Core.Office;
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

namespace PM.Base
{
    public class ExportFormWord : IHttpActionResult
    {
        private int ChangeID { get; set; }

        private string _TemplateFile { get; set; }

        private string _AttachName { get; set; }
        
        public ExportFormWord(string templateName,string attachName)
        {
            this._TemplateFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin",""), "FormFile\\"+templateName);
            this._AttachName = attachName;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            //提供数据源
            String[] fieldNames = new String[] { "EngineeringName", "EngineeringNumber" };
            Object[] fieldValues = new Object[] { "1号项目", "01X-20160106-001X" };

            var response = new HttpResponseMessage();

            var stream = WordGenerator.FillStream(_TemplateFile, fieldNames, fieldValues);

            response.Content = new ByteArrayContent(stream.ToArray());

            // 在浏览器中显示 inline
            // 附件 attachment
            ContentDispositionHeaderValue disposition = new ContentDispositionHeaderValue("attachment");
            // 写入文件基本信息
            disposition.FileName = _AttachName;
            disposition.Name = _AttachName;
            disposition.Size = stream.Length;

            response.Content.Headers.ContentDisposition = disposition;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/msword");

            // 设置缓存信息，该部分可以没有，该部分主要是用于与开始部分结合以便浏览器使用304缓存
            // Set Cache
            response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now).AddHours(1);
            // 这里应该写入文件的存储日期
            response.Content.Headers.LastModified = new DateTimeOffset(DateTime.Now);
            response.Headers.CacheControl = new CacheControlHeaderValue() { Public = true, MaxAge = TimeSpan.FromHours(1) };
            // 设置Etag，这里就简单采用 Id
            response.Headers.ETag = new EntityTagHeaderValue(string.Format("\"{0}\"", ChangeID));

            response.StatusCode = HttpStatusCode.OK;

            return await Task.FromResult(response);
        }
    }
}
