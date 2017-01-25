using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public class SysAttachFileInfo
    {
        public int ID { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime UploadDate { get; set; }

        public int Type { get; set; }

        public int UploadUser { get; set; }
    }
}
