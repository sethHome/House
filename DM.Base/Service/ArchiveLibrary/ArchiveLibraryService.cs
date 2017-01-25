using Api.Framework.Core;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.Config;
using DM.Base.Entity;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArchiveLibraryService : IArchiveLibraryService
    {
        private string _BusinessKey;

        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Dependency]
        public IArchiveNodeService _IArchiveNodeService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        public ArchiveLibraryService()
        {
            this._BusinessKey = ConstValue.BusinessKey;
        }

        public List<FondInfo> GetAll(bool WithField = false, bool WithCategory = false)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds", _BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key, true);

            var result = new List<FondInfo>();

            nodes.ForEach(n =>
            {
                var fond = new FondInfo()
                {
                    Name = n.NodeValue,
                    Number = n.NodeName,
                    Archives = new List<ArchiveInfo>()
                };

                var archives = n.ChildNodes.SingleOrDefault(c => c.NodeName == "Archive");
                if (archives != null)
                {
                    foreach (var archive in archives.ChildNodes)
                    {
                        var archiveInfo = new ArchiveInfo()
                        {
                            Key = archive.NodeName,
                            Name = archive.NodeValue,
                        };

                        var proj = archive.ChildNodes.SingleOrDefault(c => c.NodeName == "Project");
                        var vol = archive.ChildNodes.SingleOrDefault(c => c.NodeName == "Volume");
                        var catey = archive.ChildNodes.SingleOrDefault(c => c.NodeName == "Category");
                        var note = archive.ChildNodes.SingleOrDefault(c => c.NodeName == "Note");
                        var disable = archive.ChildNodes.SingleOrDefault(c => c.NodeName == "Disable");

                        archiveInfo.HasProject = proj == null ? false : proj.NodeValue == "1";
                        archiveInfo.HasVolume = vol == null ? false : vol.NodeValue == "1";
                        archiveInfo.HasCategory = catey == null ? false : catey.NodeValue == "1";
                        archiveInfo.Note = note == null ? "" : note.NodeValue;
                        archiveInfo.Disabled = disable != null;

                        if (WithCategory && archiveInfo.HasCategory)
                        {
                            archiveInfo.Categorys = GetCategorys(catey.ChildNodes);
                        }

                        if (WithField)
                        {
                            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, fond.Number, archiveInfo.Key);
                            archiveInfo.FileFields = _IFieldService.GetFields(k, "File");

                            if (archiveInfo.HasVolume)
                            {
                                k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, fond.Number, archiveInfo.Key);
                                archiveInfo.VolumeFields = _IFieldService.GetFields(k, "Volume");
                            }
                            else
                            {
                                k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Box.Field.", ConstValue.BusinessKey, fond.Number, archiveInfo.Key);
                                archiveInfo.BoxFields = _IFieldService.GetFields(k, "Box");
                            }

                            if (archiveInfo.HasProject)
                            {
                                k = string.Format("BusinessSystem.{0}.Field.Project", ConstValue.BusinessKey);
                                archiveInfo.ProjectFields = _IFieldService.GetFields(k, "Project");
                            }
                        }

                        fond.Archives.Add(archiveInfo);
                    }
                }


                result.Add(fond);
            });

            return result;
        }

        private List<CategoryInfo> GetCategorys(List<ConfigNode> nodes)
        {
            var result = new List<CategoryInfo>();

            nodes.ForEach(n =>
            {
                var info = new CategoryInfo()
                {
                    Number = n.NodeName,
                    Name = n.NodeValue,
                };

                if (n.ChildNodes.Count > 0)
                {
                    info.Children = GetCategorys(n.ChildNodes);
                }

                result.Add(info);
            });

            return result;
        }

        public ArchiveInfo GetArchiveInfo(string Fonds, string Key, bool WithField = false,bool WithCategory = false, bool WithMapping = false)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}", _BusinessKey, Fonds, Key);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key, true, 6);

            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }

            var result = new ArchiveInfo();

            result.Name = nodes[0].NodeValue;
            result.FondsNumber = Fonds;
            result.Key = Key;

            var proj = nodes.SingleOrDefault(c => c.NodeName == "Project");
            var vol = nodes.SingleOrDefault(c => c.NodeName == "Volume");
            var catey = nodes.SingleOrDefault(c => c.NodeName == "Category");
            var note = nodes.SingleOrDefault(c => c.NodeName == "Note");
            var disable = nodes.SingleOrDefault(c => c.NodeName == "Disable");

            result.HasProject = proj == null ? false : proj.NodeValue == "1";
            result.HasVolume = vol == null ? false : vol.NodeValue == "1";
            result.HasCategory = catey == null ? false : catey.NodeValue == "1";
            result.Note = note == null ? "" : note.NodeValue;
            result.Disabled = disable != null;

            if (WithCategory && result.HasCategory)
            {
                result.Categorys = GetCategorys(catey.ChildNodes);
            }

            if (WithField)
            {
                var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, Fonds, Key);
                result.FileFields = _IFieldService.GetFields(k, "File", WithMapping);

                if (result.HasVolume)
                {
                    k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, Fonds, Key);
                    result.VolumeFields = _IFieldService.GetFields(k, "Volume", WithMapping);
                }
                else
                {
                    k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Box.Field.", ConstValue.BusinessKey, Fonds, Key);
                    result.BoxFields = _IFieldService.GetFields(k, "Box", WithMapping);
                }

                if (result.HasProject)
                {
                    k = string.Format("BusinessSystem.{0}.Field.Project", ConstValue.BusinessKey);
                    result.ProjectFields = _IFieldService.GetFields(k, "Project", WithMapping);
                }
            }

            return result;
        }

        public string GetArchiveName(string FondsNumber, string ArchiveType)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}", _BusinessKey, FondsNumber, ArchiveType);

            var config = _IBaseConfig.GetConfig(key);

            return config.Value;

        }

        public int Create(ArchiveInfo Archive)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.", _BusinessKey, Archive.FondsNumber);

            if (string.IsNullOrEmpty(Archive.Name))
            {
                throw new Exception("档案馆名称不能为空");
            }

            if (CheckName(Archive.FondsNumber, Archive.Name))
            {
                throw new Exception("档案馆名称重复");
            }

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key, true, 1);
            var index = 1;
            if (nodes.Count > 0)
            {
                index = nodes.Max(n => int.Parse(n.NodeName)) + 1;
            }
            Archive.Key = index.ToString();

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}{1}", key, index),
                Value = Archive.Name,
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            if (Archive.HasVolume)
            {
                // 案卷
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}{1}.Volume", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });

                addFields(Archive, "Volume");

                // 项目,有案卷才会有项目
                if (Archive.HasProject)
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = string.Format("{0}{1}.Project", key, index),
                        Value = "1",
                        IsDeleted = false,
                        Tag = null,
                        Type = "1"
                    });

                    addFields(Archive, "Project");
                }
            }
            else
            {
                // 没有案卷，则生成盒
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}{1}.Box", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });

                addFields(Archive, "Box");
            }

            // 文件
            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}{1}.File", key, index),
                Value = "1",
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            addFields(Archive, "File");

            if (Archive.HasCategory)
            {
                // 分类表
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}{1}.Category", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            if (!string.IsNullOrEmpty(Archive.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}{1}.Note", key, index),
                    Value = Archive.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            // 新建档案类型的时候自动生成一个对应的档案节点
            _IArchiveNodeService.AddNode(new ArchiveNodeInfo()
            {
                FondsNumber = Archive.FondsNumber,
                Number = "N" + Archive.Key,
                Name = Archive.Name,
                Note = Archive.Note,
                NodeType = ArchiveNodeType.Archive,
                ArchiveType = index.ToString(),
            });

            return index;
        }

        public bool CheckName(string Fonds, string Name)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.", _BusinessKey, Fonds);

            return _IBaseConfig.Exists(c => c.Key.StartsWith(key) && c.Value == Name);
        }

        public void Update(ArchiveInfo Archive)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}", _BusinessKey, Archive.FondsNumber, Archive.Key);

            var configEntity = _IBaseConfig.GetConfig(key);

            if (configEntity.Value != Archive.Name)
            {
                configEntity.Value = Archive.Name;
                _IBaseConfig.Update(configEntity);


            }

            var hasProject = _IBaseConfig.GetConfig(string.Format("{0}.Project", key));
            var hasVolume = _IBaseConfig.GetConfig(string.Format("{0}.Volume", key));
            var hasCategory = _IBaseConfig.GetConfig(string.Format("{0}.Category", key));
            var note = _IBaseConfig.GetConfig(string.Format("{0}.Note", key));

            if (hasProject == null && Archive.HasProject)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Project", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            if (hasVolume == null && Archive.HasVolume)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Volume", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            if (hasCategory == null && Archive.HasCategory)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Category", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            if (note == null && !string.IsNullOrEmpty(Archive.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Note", key),
                    Value = Archive.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (hasProject != null)
            {
                hasProject.Value = Archive.HasProject ? "1" : "0";
                _IBaseConfig.Update(hasProject);

            }
            if (hasVolume != null)
            {
                hasVolume.Value = Archive.HasVolume ? "1" : "0";
                _IBaseConfig.Update(hasVolume);
            }
            if (hasCategory != null)
            {
                hasCategory.Value = Archive.HasCategory ? "1" : "0";
                _IBaseConfig.Update(hasCategory);
            }
            if (note != null)
            {
                note.Value = Archive.Note;
                _IBaseConfig.Update(note);
            }

            // 更新档案类型也同时更新对应的档案节点信息
            _IArchiveNodeService.UpdateNode(new ArchiveNodeInfo()
            {
                FondsNumber = Archive.FondsNumber,
                Number = "N" + Archive.Key,
                Name = Archive.Name,
                Note = Archive.Note,
                NodeType = ArchiveNodeType.Archive,
                ArchiveType = Archive.Key,
            });
        }

        public void Disable(string Fonds, string Name)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}", _BusinessKey, Fonds, Name);
            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.Disable", key),
                Value = "true",
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            // 禁用对应的档案节点
            _IArchiveNodeService.DisableANodeByArchiveType(Fonds, Name);
        }

        public void Visiable(string Fonds, string Name)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Disable", _BusinessKey, Fonds, Name);
            _IBaseConfig.Delete(key);

            // 启用对应的档案节点
            _IArchiveNodeService.VisiableNodeByArchiveType(Fonds, Name);
        }

        public void AddCategory(string Fonds, string Type, CategoryInfo Info)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Category.{3}", _BusinessKey, Fonds, Type, string.IsNullOrEmpty(Info.Parent) ? Info.Number.Replace(".", "") : Info.Parent.Trim('.') + "." + Info.Number.Replace(".", ""));

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = key,
                Value = Info.Name,
                IsDeleted = false,
                Type = "1"
            });
        }

        public void UpdateCategory(string Fonds, string Type, string Number, CategoryInfo Info)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Category.{3}.{4}", _BusinessKey, Fonds, Type, Info.Parent, Number);

            var config = _IBaseConfig.GetConfig(key);

            config.Key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Category.{3}.{4}", _BusinessKey, Fonds, Type, Info.Parent, Info.Number);
            config.Value = Info.Name;

            _IBaseConfig.Update(config);
        }

        public void DeleteCategory(string Fonds, string Type, string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Category.{3}", _BusinessKey, Fonds, Type, FullKey);

            _IBaseConfig.Delete(key);
        }

        public void Delete(string Fonds, string Name)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}", _BusinessKey, Fonds, Name);
            _IBaseConfig.Delete(c => c.Key.StartsWith(key));

            // 删除对应的档案节点
            _IArchiveNodeService.DeleteNodeByArchiveType(Fonds, Name);

            // 删除档案的数据库
            // todo
        }

        private void addFields(ArchiveInfo archive, string key)
        {
            var k = string.Format("BusinessSystem.{0}.Field.{1}", ConstValue.BusinessKey, key);

            var fields = _IFieldService.GetFields(k, key);

            foreach (var item in fields)
            {
                item.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.", ConstValue.BusinessKey, archive.FondsNumber, archive.Key, key);
                _IFieldService.AddField(item);
            }
        }

        public void Generate(string Fonds, string Archive)
        {
            //var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, Fonds, Archive);
            //var volumeFields = _IFieldService.GetFields(k, "Volume");

            //k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Project.Field.", ConstValue.BusinessKey, Fonds, Archive);
            //var projectFields = _IFieldService.GetFields(k, "Project");

            //k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, Fonds, Archive);
            //var fileFields = _IFieldService.GetFields(k, "File");

            //k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Box.Field.", ConstValue.BusinessKey, Fonds, Archive);
            //var boxFields = _IFieldService.GetFields(k, "Box");


            var archiveInfo = this.GetArchiveInfo(Fonds, Archive, true);

            var volumeFields = archiveInfo.VolumeFields;
            var fileFields = archiveInfo.FileFields;
            var boxFields = archiveInfo.BoxFields;

            var dataTypes = _IEnum.GetEnumInfo(ConstValue.BusinessKey, "FieldType");

            var dataTypeToSqlMap = new Dictionary<int, string>();

            foreach (var item in dataTypes.Items)
            {
                dataTypeToSqlMap.Add(int.Parse(item.Key), item.Tags["sqldatatype"]);
            }

            if (archiveInfo.HasVolume)
            {
                createOrAlterTable(string.Format("ArchiveDoc_{0}_{1}_Volume", Fonds, Archive), "Volume", volumeFields, dataTypeToSqlMap, archiveInfo.HasProject,archiveInfo.HasCategory);
            }
            else
            {
                createOrAlterTable(string.Format("ArchiveDoc_{0}_{1}_Box", Fonds, Archive), "Box", boxFields, dataTypeToSqlMap, archiveInfo.HasProject, archiveInfo.HasCategory);
            }

            createOrAlterTable(string.Format("ArchiveDoc_{0}_{1}_File", Fonds, Archive), "File", fileFields, dataTypeToSqlMap);
        }

        public void GenerateProject(string Fonds)
        {
            var k = string.Format("BusinessSystem.{0}.Field.Project", ConstValue.BusinessKey);

            var projectFields = _IFieldService.GetFields(k, "Project");

            var dataTypes = _IEnum.GetEnumInfo(ConstValue.BusinessKey, "FieldType");

            var dataTypeToSqlMap = new Dictionary<int, string>();

            foreach (var item in dataTypes.Items)
            {
                dataTypeToSqlMap.Add(int.Parse(item.Key), item.Tags["sqldatatype"]);
            }

            if (projectFields.Count > 0)
            {
                createOrAlterTable("Project", "Project", projectFields, dataTypeToSqlMap);
            }
        }

        private void createOrAlterTable(string tableName, string Name, List<FieldInfo> fields, Dictionary<int, string> dataTypeToSqlMap, bool hasProject = false, bool hasCategory = false)
        {
            var context = new DMContext();

            var exists = context.Database.SqlQueryForDataTatable(string.Format(@"SELECT TOP 1 1 FROM SYSOBJECTS WHERE xtype='u' AND name = '{0}'", tableName));

            if (exists.Rows.Count == 1)
            {
                // 如果表没有数据直接删除表重建
                var tableSource = context.Database.SqlQueryForDataTatable(string.Format(@"SELECT TOP 1 1 FROM dbo.[{0}]", tableName));

                if (tableSource.Rows.Count == 0)
                {
                    dropTable(context, tableName);

                    createTable(context, Name, tableName, fields, dataTypeToSqlMap, hasProject, hasCategory);
                }
                else
                {
                    alterTable(context, tableName, fields, dataTypeToSqlMap);
                }
            }
            else
            {
                createTable(context, Name, tableName, fields, dataTypeToSqlMap, hasProject, hasCategory);
            }
        }

        private void dropTable(DMContext context, string tableName)
        {
            context.Database.ExecuteSqlCommand(string.Format(@"DROP TABLE dbo.[{0}]", tableName));
        }

        private void createTable(DMContext context, string Name, string tableName, List<FieldInfo> fields, Dictionary<int, string> dataTypeToSqlMap, bool hasProject = false, bool hasCategory = false)
        {
            var sbSql = new StringBuilder();

            if (Name == "File")
            {
                sbSql.Append(@"[NodeID] [nvarchar](50) NULL,
                    [RefID] [INT] NULL,
                    [FileNumber] [INT] NULL,
                    [FileID] [INT] NULL,");
            }
            else if (Name == "Project")
            {

            }
            else
            {
                sbSql.Append(@"[NodeID] [nvarchar](50) NULL,
                    [FileNumber] [INT] NULL,
                    [AccessLevel] [INT] NOT NULL DEFAULT(1),
                    [Copies] [INT] NOT NULL DEFAULT(1),");

                if (hasProject)
                {
                    sbSql.Append(@"[ProjectID] [INT] NOT NULL DEFAULT(0),");
                }

                if (hasCategory)
                {
                    sbSql.Append(@"[Category] [NVARCHAR](50) NULL,");
                }
            }

            foreach (var field in fields)
            {
                var defaultStr = "";

                if (field.DataType == 1 || field.DataType == 5)
                {
                    defaultStr = string.Format("DEFAULT({0}) ", field.GetDefaultValue());
                }
                else
                {
                    defaultStr = string.Format("DEFAULT('{0}') ", field.GetDefaultValue());
                }

                sbSql.AppendFormat("_f{0} {1}{2} {3},",
                    field.ID, dataTypeToSqlMap[field.DataType],
                    field.Length > 0 ? string.Format("({0})", field.Length) : "",
                    field.NotNull ? string.Format("NOT NULL {0} ", defaultStr) : "");
            }

            var sql = string.Format(@"
                CREATE TABLE [dbo].[{0}](
	                [ID] [int] IDENTITY(1,1) NOT NULL,
                    [FondsNumber] [nvarchar](20) NOT NULL,
	                {1}
                    [Status] [int] NOT NULL DEFAULT(1),
                    [CreateUser] [int] NOT NULL DEFAULT(0),
                    [CreateDate] [datetime] NOT NULL DEFAULT(GETDATE()),
                    [ModifyUser] [int] NULL,
                    [ModifyDate] [datetime] NULL,
                    [IsDelete] [bit] NOT NULL DEFAULT(0),

                    CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                    (
	                    [ID] ASC
                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]", tableName, sbSql.ToString());

            context.Database.ExecuteSqlCommand(sql);
        }

        private void alterTable(DMContext context, string tableName, List<FieldInfo> fields, Dictionary<int, string> dataTypeToSqlMap)
        {
            var result = new StringBuilder();

            var columnSql = string.Format(@"SELECT CONVERT(INT,SUBSTRING(name,3,2),0) AS name FROM SYSCOLUMNS C
                WHERE C.name like '_f%' AND C.name <> 'FondsNumber'
                AND EXISTS(SELECT TOP 1 1 FROM SYSOBJECTS T WHERE C.id = T.id AND T.xtype='u' AND T.name = '{0}' )", tableName);

            var columns = context.Database.SqlQueryForDataTatable(columnSql);

            var fieldIDs = string.Join(",", fields.Select(f => f.ID));

            // 需要被删除的字段
            var dropCols = columns.Select(string.Format("name NOT IN ({0})", fieldIDs));

            // 删除列
            if (dropCols.Length > 0)
            {
                var cols = string.Join(",", dropCols.Select(c => "_f" + c[0].ToString()));

                var sqlDropConSql = string.Format(@"SELECT B.name FROM SYSOBJECTS B JOIN SYSCOLUMNS A ON B.id = A.cdefault 
                    WHERE A.id = OBJECT_ID('{0}') 
                    AND A.name IN ('{1}')", tableName, string.Join("','", dropCols.Select(c => "_f" + c[0].ToString())));

                var cons = context.Database.SqlQueryForDataTatable(sqlDropConSql);
                if (cons.Rows.Count > 0)
                {
                    var conStrs = cons.Select().Select(c => c[0].ToString());
                    result.AppendFormat(@"ALTER TABLE {0} DROP CONSTRAINT {1}", tableName, string.Join(",", conStrs));
                }

                result.AppendFormat(@"
                    ALTER TABLE {0} DROP COLUMN {1}", tableName, string.Join(",", dropCols.Select(c => "_f" + c[0].ToString())));
            }

            var existsCols = columns.Select(string.Format("name IN ({0})", fieldIDs)).Select(r => r[0].ToString());

            // 需要新增的字段
            var newCols = fields.Where(f => !existsCols.Contains(f.ID));

            if (newCols.Count() > 0)
            {
                result.AppendFormat(@"
                ALTER TABLE {0} ADD ", tableName);

                foreach (var field in newCols)
                {
                    var defaultStr = "";

                    if (field.DataType == 1 || field.DataType == 5)
                    {
                        defaultStr = string.Format("DEFAULT({0}) ", field.GetDefaultValue());
                    }
                    else
                    {
                        defaultStr = string.Format("DEFAULT('{0}') ", field.GetDefaultValue());
                    }

                    result.AppendFormat("_f{0} {1}{2} {3},",
                        field.ID, dataTypeToSqlMap[field.DataType],
                        field.Length > 0 ? string.Format("({0})", field.Length) : "",
                        field.NotNull ? string.Format("NOT NULL {0}", defaultStr) : "");
                }
            }

            if (dropCols.Length > 0 || newCols.Count() > 0)
            {
                context.Database.ExecuteSqlCommand(result.ToString().TrimEnd(','));
            }
        }
    }
}
