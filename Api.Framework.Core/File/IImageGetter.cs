using Api.Framework.Core.Attach;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public interface IImageGetter
    {
        SysAttachFileEntity GetAttach(int ID);

        SysAttachFileEntity SaveImage(string FileName, string MediaType, string Name,string Md5Str, FileInfo ImageFile);

        List<SysAttachInfo> GetAttachs(string IDs,bool withTags = false);

        List<SysAttachInfo> GetAttachs(List<int> IDs, bool withTags = false);

        SysAttachFileEntity Check(string Md5Code);

        int Add(FileInfo File);
    }
}
