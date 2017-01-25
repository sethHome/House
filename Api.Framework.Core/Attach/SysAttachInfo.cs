using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Attach
{
    public class SysAttachInfo
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

        /// <summary>
        /// 自定义文件所属目录（批量压缩文件时用到此属性，以便生成相应的文件结构）
        /// </summary>
        public string CustDirectory { get; set; }

        public SysAttachInfo(SysAttachFileEntity Entity)
        {
            this.ID = Entity.ID;
            this.Path = Entity.Path;
            this.Name = Entity.Name;
            this.Size = Entity.Size;
            this.UploadDate = Entity.UploadDate;
            this.FileDate = Entity.FileDate;
            this.Type = Entity.Type;
            this.UploadUser = Entity.UploadUser;
            this.Extension = Entity.Extension;
        }

        public List<SysTagEntity> Tags { get; set; }
    }
}
