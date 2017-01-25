using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public class UploadResponseModel
    {
        public List<FileResponseModel> files;

        public UploadResponseModel()
        {
            files = new List<FileResponseModel>();
        }
    }
}
