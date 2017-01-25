using Api.Framework.Core.Attach;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.Tag;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.File
{
    public class DBImageGetter : BaseService<SysAttachFileEntity>, IImageGetter
    {
        [Dependency]
        public IEnum _IEnum { get; set; }

        [Dependency]
        public IObjectTagService _IObjectTagService { get; set; }

        public SysAttachFileEntity SaveImage(string FileName, string MediaType, string Name, string Md5Str, FileInfo ImageFile)
        {
            var entity = new SysAttachFileEntity()
            {
                Name = FileName,
                Path = ImageFile.FullName,
                Size = ImageFile.Length,
                Extension = ImageFile.Extension,

                UploadDate = DateTime.Now,
                FileDate = DateTime.Now,
                Type = getAttachType(MediaType, FileName),
                UploadUser = string.IsNullOrEmpty(Name) ? 0 : int.Parse(Name),
             
                Md5 = Md5Str
            };

            base.DB.Add(entity);

            return entity;
        }

        private int getAttachType(string MediaType, string FileName)
        {
            //application/octet-stream
            if (MediaType == "application/octet-stream")
            {
                var ex = getFileExtension(FileName).ToLower();

                if (ex == "rar")
                {
                    return (int)EnumAttachType.RAR;
                }
                else if (ex == "zip")
                {
                    return (int)EnumAttachType.Zip;
                }
            }

            var attachTypes = _IEnum.GetEnumInfo("System3", "AttachType");
            var otherValue = ((int)EnumAttachType.Other).ToString();
            foreach (var item in attachTypes.Items)
            {
                if (item.Value != otherValue
                    && MediaType.Contains(item.Tags["media"]))
                {
                    return int.Parse(item.Value);
                }
            }

            return (int)EnumAttachType.Other;
        }

        private string getFileExtension(string FileNmae)
        {
            var index = FileNmae.LastIndexOf('.');
            return FileNmae.Substring(index + 1);
        }

        public List<SysAttachInfo> GetAttachs(string IDs, bool withTags = false)
        {
            if (IDs != null && IDs.Length > 0)
            {
                var idStrs = IDs.Split(',');
                var ids = new int[idStrs.Length];
                for (int i = 0; i < idStrs.Length; i++)
                {
                    ids[i] = int.Parse(idStrs[i]);
                }

                var list = base.DB.GetList(a => ids.Contains(a.ID)).ToList();

                var resut = new List<SysAttachInfo>();

                foreach (var item in list)
                {
                    resut.Add(new SysAttachInfo(item)
                    {
                        Tags = withTags ? _IObjectTagService.GetObjectTags("Attach", item.ID) : null
                    });

                }

                return resut;
            }
            else
            {
                return new List<SysAttachInfo>();
            }

        }

        public List<SysAttachInfo> GetAttachs(List<int> IDs, bool withTags = false)
        {
            var list = base.DB.GetList(a => IDs.Contains(a.ID)).ToList();

            var resut = new List<SysAttachInfo>();

            foreach (var item in list)
            {
                resut.Add(new SysAttachInfo(item)
                {
                    Tags = withTags ? _IObjectTagService.GetObjectTags("Attach", item.ID) : null
                });

            }

            return resut;
        }

        public SysAttachFileEntity GetAttach(int ID)
        {
            return base.DB.Get(ID);
        }

        public SysAttachFileEntity Check(string Md5Code)
        {
            return base.DB.SingleOrDefault(a => a.Md5.Equals(Md5Code));
        }

        public int Add(FileInfo File)
        {
            var entity = new SysAttachFileEntity()
            {
                Name = File.Name,
                Path = File.FullName,
                Size = File.Length,
                Extension = File.Extension,

                UploadDate = DateTime.Now,
                FileDate = DateTime.Now,
                Type = (int)EnumAttachType.Other,
                UploadUser = 0,

                Md5 = ""
            };

            base.DB.Add(entity);

            return entity.ID;
        }
    }
}
