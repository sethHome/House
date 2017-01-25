using Api.Framework.Core.Attach;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
   public class ZipFileHelp
    {
        public static FileStream ZipFileByCode(List<SysAttachInfo> Attachs)
        {
            var dic = new Dictionary<string, string>();
            var ids = new StringBuilder();
            foreach (var attach in Attachs)
            {
                dic.Add(attach.Path, string.IsNullOrEmpty(attach.CustDirectory) ?  attach.Name : attach.CustDirectory);

                ids.AppendFormat("{0}-", attach.ID);
            }

            var downloadTempPath = System.Configuration.ConfigurationManager.AppSettings["DownloadTempPath"];

            var filePath = string.Format(@"{0}\\{1}.zip", downloadTempPath, ids.ToString().TrimEnd('-'));

            if (System.IO.File.Exists(filePath))
            {
                return new FileStream(filePath, FileMode.Open);
            }

            MemoryStream ms = new MemoryStream();
           
            using (ZipFile file = ZipFile.Create(ms))
            {
                file.BeginUpdate();
                file.NameTransform = new MyNameTransfom(dic);//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。

                foreach (var attach in Attachs)
                {
                    file.Add(attach.Path);
                }
                
                file.CommitUpdate();
            }
            
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(ms.ToArray());
            ms.Close();
            fs.Position = 0;
            return fs;
        }

        
    }

    public class MyNameTransfom : INameTransform
    {
        private Dictionary<string, string> _PathName;

        public MyNameTransfom(Dictionary<string, string> PathName)
        {
            this._PathName = PathName;
        }

        public string TransformDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public string TransformFile(string name)
        {
            if (this._PathName.ContainsKey(name)) {
                return this._PathName[name];
            }

            return name;
        }
    }
}
