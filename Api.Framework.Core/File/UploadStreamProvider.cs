using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Api.Framework.Core.File
{
    public class UploadStreamProvider : MultipartFormDataStreamProvider
    {
        private string guid;

        public string FileName { get; set; }

        public string MediaType { get; set; }

        public DateTime ModifyDate { get; set; }

        public string FullPath { get; set; }

        public UploadStreamProvider(string uploadPath)
            : base(uploadPath)
        {
            //guid = Guid.NewGuid().ToString();
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            this.MediaType = headers.ContentType.MediaType;
            //this.ModifyDate = headers.ContentDisposition.ModificationDate.Value;
            
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                this.FileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                return getFilePath(this.FileName);
            }

            return guid;
        }


        private string getFilePath(string filename)
        {
            var filePath = Path.Combine(base.RootPath, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            string directory = base.RootPath;
            string name = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFullPath;
            do
            {
                string newFilename = string.Format("{0}({1}){2}", name, counter, extension);
                newFullPath = Path.Combine(directory, newFilename);
                counter++;
            } while (System.IO.File.Exists(newFullPath));

            return newFullPath;
        }


    }
}
