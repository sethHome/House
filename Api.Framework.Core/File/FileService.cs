using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public class FileService : BaseService<SysAttachFileEntity>, IFileService
    {
        public void Add(SysAttachFileEntity File)
        {
            base.DB.Add(File);
        }
    }
}
