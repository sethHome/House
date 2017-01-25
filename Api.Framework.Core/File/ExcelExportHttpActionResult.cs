using Api.Framework.Core;
using Api.Framework.Core.Office;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class ExcelExportHttpActionResult : IHttpActionResult
    {
        private DataTable _Source;

        private string _Title;

        private Dictionary<string, ColumnMapInfo> _MapInfos;

        public ExcelExportHttpActionResult(DataTable Source,string title = "未命名", Dictionary<string, ColumnMapInfo> mapInfos = null)
        {
            this._Source = Source;
            this._Title = title;
            this._MapInfos = mapInfos;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage();

            if (_Source != null)
            {
                ExcelGenerator.ExportToFile(_Source, _Title, "D:\\text.xls");

                MemoryStream ms = ExcelGenerator.Generate(this._Source, this._Title, this._MapInfos);

                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                response.Content.Headers.ContentDisposition.FileName = string.Format("{0}.xls", _Title);
                response.Content.Headers.LastModified = new DateTimeOffset(DateTime.Now);
                //response.Content.Headers.ContentLength = ms.Length;
                response.StatusCode = HttpStatusCode.OK;
            }

            return await Task.FromResult(response);
        }
    }
}
