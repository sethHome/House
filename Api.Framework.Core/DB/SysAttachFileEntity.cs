using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class SysAttachFileEntity
    {
        public int ID { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime FileDate { get; set; }

        public int Type { get; set; }

        public int UploadUser { get; set; }

        public string Extension { get; set; }

        public string Md5 { get; set; }

    }
}
