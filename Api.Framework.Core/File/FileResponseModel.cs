using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public class FileResponseModel
    {
        public int id;
        public string name;
        public long size;
        public string type;
        public string url;
        public string error;
        public string deleteUrl;
        public string deleteType;
        public string upDateTime;
        public string mediaType;
    }
}
