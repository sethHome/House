using Api.Framework.Core;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.Config;
using DM.Base.Entity;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class FileLibraryService : IFileLibraryService
    {
        
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        public List<FileLibraryInfo> GetAll()
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds", ConstValue.BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key);

            var result = new List<FileLibraryInfo>();

            nodes.ForEach(n =>
            {
                var fond = new FileLibraryInfo()
                {
                    ID = n.NodeName,
                    Name = n.NodeValue,
                    Number = n.NodeName,
                    FondsNumber = n.NodeName,
                    NodeType = 0
                };

                var fileLibrary = n.ChildNodes.FirstOrDefault(c => c.NodeName == "FileLibrary");

                fond.Children = getChildren(fond.Number, "", fileLibrary.ChildNodes);

                result.Add(fond);
            });

            return result;
        }

        private List<FileLibraryInfo> getChildren(string fondNumber,string parentID, IEnumerable<ConfigNode> nodes)
        {
            var result = new List<FileLibraryInfo>();

            foreach (var node in nodes)
            {
                var archive = new FileLibraryInfo()
                {
                    ID = parentID == "" ? node.NodeName : string.Format("{0}.{1}",parentID, node.NodeName),
                    Number = node.NodeName,
                    Name = node.NodeValue,
                    FondsNumber = fondNumber,
                    NodeType = string.IsNullOrEmpty(parentID) ? 1 : 2
                };

                var noteNode = node.ChildNodes.SingleOrDefault(n => n.NodeName == "Note");
              
                if (noteNode != null)
                {
                    archive.Note = noteNode.NodeValue;
                }

                var childNode = node.ChildNodes.FirstOrDefault(n => n.NodeName == "Node");
                if (childNode != null && childNode.ChildNodes != null)
                {
                    archive.Children = getChildren(fondNumber, archive.ID, childNode.ChildNodes);
                }

                result.Add(archive);
            }

            return result;
        }

        public int Create(FileLibraryInfo FileLibrary)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}", 
                ConstValue.BusinessKey, FileLibrary.FondsNumber, FileLibrary.ParentFullKey);

            if (string.IsNullOrEmpty(FileLibrary.Name))
            {
                throw new Exception("文件库名称不能为空");
            }

            if (CheckName(FileLibrary))
            {
                throw new Exception("文件库名称重复");
            }

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key, true, 1);
            var index = 1;
            if (nodes.Count > 0)
            {
                index = nodes.Max(n => int.Parse(n.NodeName)) + 1;
            }
            FileLibrary.Number = index.ToString();

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}{1}", key, index),
                Value = FileLibrary.Name,
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            if (!string.IsNullOrEmpty(FileLibrary.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}{1}.Note", key, index),
                    Value = FileLibrary.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            return index;
        }

        public bool CheckName(FileLibraryInfo FileLibrary)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.", ConstValue.BusinessKey, FileLibrary.FondsNumber, FileLibrary.ParentFullKey);

            return _IBaseConfig.Exists(c => c.Key.StartsWith(key) && c.Value == FileLibrary.Name);
        }

        public void Update(FileLibraryInfo FileLibrary)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}", ConstValue.BusinessKey, FileLibrary.FondsNumber, FileLibrary.Number);

            if (!string.IsNullOrEmpty(FileLibrary.ParentFullKey))
            {
                key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}{3}", ConstValue.BusinessKey, FileLibrary.FondsNumber, FileLibrary.ParentFullKey, FileLibrary.Number);
            }

            var configEntity = _IBaseConfig.GetConfig(key);

            if (configEntity.Value != FileLibrary.Name)
            {
                configEntity.Value = FileLibrary.Name;
                _IBaseConfig.Update(configEntity);
            }
          
            var note = _IBaseConfig.GetConfig(string.Format("{0}.Note", key));

            if (note == null && !string.IsNullOrEmpty(FileLibrary.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Note", key),
                    Value = FileLibrary.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (note != null)
            {
                note.Value = FileLibrary.Note;
                _IBaseConfig.Update(note);
            }
        }

        public void Delete(string Fonds, string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}", ConstValue.BusinessKey, Fonds, FullKey);
            _IBaseConfig.Delete(c => c.Key.StartsWith(key));
        }

        public void Generate(string Fonds, string FileNumer)
        {
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.", ConstValue.BusinessKey, Fonds, FileNumer);
            var fields =  _IFieldService.GetFields(k, "");

            var dataTypes = _IEnum.GetEnumInfo(ConstValue.BusinessKey, "FieldType");

            var dataTypeToSqlMap = new Dictionary<int, string>();

            foreach (var item in dataTypes.Items)
            {
                dataTypeToSqlMap.Add(int.Parse(item.Key), item.Tags["sqldatatype"]);
            }

            var sbSql = new StringBuilder();

            foreach (var field in fields)
            {
                sbSql.AppendFormat("_f{0} {1}{2} {3},",
                    field.ID, dataTypeToSqlMap[field.DataType], 
                    field.Length > 0 ? string.Format("({0})", field.Length) : "",
                    field.NotNull ? "NOT NULL" : "");
            }

            var sql = string.Format(@"
                CREATE TABLE [dbo].[FileDoc_{0}_{1}](
	                [ID] [int] IDENTITY(1,1) NOT NULL,
                    [FondsNumber] [nvarchar](20) NOT NULL,
                    [NodeID] [nvarchar](50) NULL,
	                {2}
                    [Status] [int] NOT NULL DEFAULT(1),
                    [Dept] [nvarchar](50) NULL,
                    [CreateUser] [int] NOT NULL DEFAULT(0),
                    [CreateDate] [datetime] NOT NULL DEFAULT(GETDATE()),
                    [ModifyUser] [int] NULL,
                    [ModifyDate] [datetime] NULL,
                    [IsDelete] [bit] NOT NULL DEFAULT(0),

                    CONSTRAINT [PK_FileDoc_{0}_{1}] PRIMARY KEY CLUSTERED 
                    (
	                    [ID] ASC
                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]", Fonds,FileNumer, sbSql.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql);
        }
    }
}
