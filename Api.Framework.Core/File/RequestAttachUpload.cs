using Api.Framework.Core.Safe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Framework.Core.File
{
    public class RequestAttachUpload
    {
        public static async Task<UploadResponseModel> Upload(HttpRequestMessage Request, string Name)
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                var uploadPath = System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"];

                var date = DateTime.Now;

                var fullPath = Path.Combine(uploadPath, date.Year.ToString(), date.Month.ToString(), date.Day.ToString());

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var _IImageGetter = UnityContainerHelper.GetServer<IImageGetter>();
                var response = new UploadResponseModel();
                var streamProvider = new UploadStreamProvider(fullPath);
                var upResult = await Request.Content.ReadAsMultipartAsync(streamProvider);

                foreach (var file in upResult.FileData)
                {
                    var stream = new FileStream(file.LocalFileName, FileMode.Open);
                    var md5Str = Md5.GetMd5Hash(stream);
                    stream.Close();

                    FileInfo fi = new FileInfo(file.LocalFileName);
                    var entity = _IImageGetter.SaveImage(streamProvider.FileName, streamProvider.MediaType, Name, md5Str, fi);

                    // 检查文件的MD5 如果有相同的MD5文件，则直接返回已有的文件ID
                    // var entity = _IImageGetter.Check(md5Str);
                    //if (entity == null)
                    //{
                    //    FileInfo fi = new FileInfo(file.LocalFileName);
                    //    entity = _IImageGetter.SaveImage(streamProvider.FileName, streamProvider.MediaType, Name, md5Str, fi);
                    //}

                    response.files.Add(new FileResponseModel()
                    {
                        id = entity.ID,
                        name = entity.Name,
                        size = entity.Size,
                        error = "",
                        mediaType = streamProvider.MediaType,
                        type = entity.Extension,
                        upDateTime = entity.UploadDate.ToString("yyyy-MM-dd hh:mm:ss")
                    });
                }

                return response;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

        private static string getFilePath(string rootPath, string filename)
        {
            var filePath = Path.Combine(rootPath, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            string directory = rootPath;
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
