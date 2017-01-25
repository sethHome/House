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
    public class EditorImageActionResult : IHttpActionResult
    {

        private UploadResponseModel _FileInfo;

        public EditorImageActionResult(UploadResponseModel fileInfo)
        {
            _FileInfo = fileInfo;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            //string data = "asdfasdfs";
            //string script = "(function(){alert('" + data + "');})();";
            byte[] bytes = Encoding.UTF8.GetBytes("http://localhost:8002/api/v1/image/"+ _FileInfo.files[0].id);

            response.Content = new ByteArrayContent(bytes);
            
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
            
            response.StatusCode = HttpStatusCode.OK;

            return await Task.FromResult(response);
        }
    }
}
