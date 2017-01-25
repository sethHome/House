using Api.Framework.Core;
using Api.Framework.Core.Attach;
using Api.Framework.Core.Config;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.File;
using DM.Base.Entity;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArchiveDataService : IArchiveDataService
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IFileDataService _IFileDataService { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Dependency]
        public IArchiveLibraryService _IArchiveLibraryService { get; set; }

        [Dependency]
        public IArchiveLogService _IArchiveLogService { get; set; }

        [Dependency]
        public ILuceneIndexService _ILuceneIndexService { get; set; }

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IImageGetter _IImageGetter { get; set; }

        /// <summary>
        /// 自动组卷
        /// </summary>
        public int AutoCreate(AutoCreateArchiveInfo CreateInfo)
        {
            DataTable files = null;

            if (string.IsNullOrEmpty(CreateInfo.IDs))
            {
                var resule = _IFileDataService.GetFileData(CreateInfo.FondsNumber, CreateInfo.FileNumber, CreateInfo.Param, CreateInfo.Conditions);

                files = resule.Source;

                // 将文件库内的文件设置为已归档
                _IFileDataService.SetFileArchived(CreateInfo.FondsNumber, CreateInfo.FileNumber, CreateInfo.Param, CreateInfo.Conditions);
            }
            else
            {
                var param = new PageQueryParam()
                {
                    OrderFiled = "ID",
                    IsDesc = true,
                    FilterCondtion = new Hashtable(),
                };

                param.FilterCondtion.Add("IDs", CreateInfo.IDs);

                var resule = _IFileDataService.GetFileData(CreateInfo.FondsNumber, CreateInfo.FileNumber, param, null);
                files = resule.Source;

                // 将文件库内的文件设置为已归档
                _IFileDataService.SetFileArchived(CreateInfo.FondsNumber, CreateInfo.FileNumber, CreateInfo.IDs);
            }


            return AutoCreate(CreateInfo, files);
        }

        public Dictionary<string, object> GetArchiveData(int ID, string FondsNumber, string ArchiveType, EnumArchiveName Name)
        {
            var sql = string.Format(@"SELECT TOP 1 * FROM [dbo].[ArchiveDoc_{1}_{2}_{3}] WHERE ID = {0}", ID, FondsNumber, ArchiveType, Name.ToString());

            var context = new DMContext();
            var table = context.Database.SqlQueryForDataTatable(sql);

            var result = new Dictionary<string, object>();

            if (table.Rows.Count > 0)
            {
                foreach (DataColumn col in table.Columns)
                {
                    result.Add(col.ColumnName, table.Rows[0][col]);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// 获取案卷
        /// </summary>
        public TableSource GetArchiveVolumes(string FondsNumber, string ArchiveType, PageQueryParam Param, ArchiveQueryInfo QueryInfo)
        {
            var sbWhereSql = new StringBuilder("WHERE [IsDelete] = 0");
            var sqlParams = new List<SqlParameter>();

            var archiveInfo = _IArchiveLibraryService.GetArchiveInfo(FondsNumber, ArchiveType);

            var orderStr = "[ID]";

            if (!string.IsNullOrEmpty(Param.OrderFiled))
            {
                orderStr = string.Format("[{0}] {1}", Param.OrderFiled, Param.IsDesc ? "DESC" : "");
            }

            foreach (DictionaryEntry filter in Param.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    case "ID":
                        {
                            sbWhereSql.Append(" AND [ID] = @ID");
                            sqlParams.Add(new SqlParameter("@ID", val));
                            break;
                        }
                    case "IDs":
                        {
                            if (!string.IsNullOrEmpty(val))
                            {
                                sbWhereSql.AppendFormat(" AND [ID] IN ({0})", val);
                            }
                           
                            break;
                        }
                    case "NodeID":
                        {
                            if (!string.IsNullOrEmpty(val))
                            {
                                sbWhereSql.Append(" AND [NodeID] = @NodeID");
                                sqlParams.Add(new SqlParameter("@NodeID", val));
                            }
                            
                            break;
                        }
                    case "Category":
                        {
                            if (!string.IsNullOrEmpty(val) && archiveInfo.HasCategory)
                            {
                                sbWhereSql.Append(" AND [Category] = @Category");
                                sqlParams.Add(new SqlParameter("@Category", val));
                            }
                          
                            break;
                        }
                    case "Status":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                sbWhereSql.Append(" AND [Status] = @Status");
                                sqlParams.Add(new SqlParameter("@Status", intVal));
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            var wheresql = (QueryInfo == null || QueryInfo.Conditions == null) ? string.Empty : QueryInfo.Conditions.ToQuerySql(sqlParams);

            if (!string.IsNullOrEmpty(wheresql))
            {
                sbWhereSql.AppendFormat(" AND {0}", wheresql);
            }

            if (QueryInfo != null && !string.IsNullOrEmpty(QueryInfo.ConditionsSqlStr))
            {
                sbWhereSql.AppendFormat(" AND ({0})", QueryInfo.ConditionsSqlStr);
            }

            // 判断是案卷还是盒
            //var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume", ConstValue.BusinessKey, FondsNumber, ArchiveType);
            //var hasVolume = _IBaseConfig.Exists(c => c.Key == k);

            if (Param.PageSize > 0)
            {
                var sql = string.Format(@"

                SELECT @AllCount = COUNT(1) FROM [dbo].[ArchiveDoc_{0}_{1}_{6}] {4}

                SELECT TOP {2} *
                FROM 
                (

                 SELECT ROW_NUMBER() OVER(ORDER BY {5}) AS ROWNUMBER,A.*,
                 (SELECT COUNT(1) FROM ArchiveBorrowItem B WITH(NOLOCK) WHERE A.ID = B.ArchiveID AND B.Fonds = '{0}' AND B.ArchiveType = '{1}' AND B.[Status] = 2) AS BorrowCount
                 FROM [dbo].[ArchiveDoc_{0}_{1}_{6}] A WITH(NOLOCK)
                 {4}
 
                ) A
                WHERE ROWNUMBER > {3}
                ORDER BY {5}", FondsNumber, ArchiveType, Param.PageSize, Param.PageSize * (Param.PageIndex - 1), sbWhereSql.ToString(), orderStr, archiveInfo.HasVolume ? "Volume" : "Box");

                var paramAllCount = new SqlParameter("@AllCount", 0) { Direction = ParameterDirection.Output };
                sqlParams.Add(paramAllCount);

                var context = new DMContext();
                var table = context.Database.SqlQueryForDataTatable(sql, sqlParams.ToArray());
                return new TableSource(table, Convert.ToInt32(paramAllCount.Value), Param.PageSize);
            }
            else
            {
                var sql = string.Format(@"
                SELECT  A.*,
                (SELECT COUNT(1) FROM ArchiveBorrowItem B WITH(NOLOCK) WHERE A.ID = B.ArchiveID AND B.Fonds = '{0}' AND B.ArchiveType = '{1}' AND B.[Status] = 2) AS BorrowCount
                FROM [dbo].[ArchiveDoc_{0}_{1}_{4}] A WITH(NOLOCK)
                {2}
                ORDER BY {3}", FondsNumber, ArchiveType, sbWhereSql.ToString(), orderStr, archiveInfo.HasVolume ? "Volume" : "Box");

                var context = new DMContext();
                var table = context.Database.SqlQueryForDataTatable(sql, sqlParams.ToArray());

                return new TableSource(table, 0, 0);
            }
        }

        /// <summary>
        /// 获取案卷文件
        /// </summary>
        public TableSource GetArchiveFiles(string FondsNumber, string ArchiveType, PageQueryParam Param, List<Condition> Conditions)
        {
            var sbWhereSql = new StringBuilder("WHERE [IsDelete] = 0");
            var sqlParams = new List<SqlParameter>();

            var orderStr = "[ID]";

            if (!string.IsNullOrEmpty(Param.OrderFiled))
            {
                orderStr = string.Format("[{0}] {1}", Param.OrderFiled, Param.IsDesc ? "DESC" : "");
            }

            foreach (DictionaryEntry filter in Param.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    case "IDs":
                        {
                            sbWhereSql.AppendFormat(" AND [ID] IN ({0})", val);
                            break;
                        }
                    case "NodeID":
                        {
                            sbWhereSql.Append(" AND [NodeID] = @NodeID");
                            sqlParams.Add(new SqlParameter("@NodeID", val));
                            break;
                        }
                    case "Status":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                sbWhereSql.Append(" AND [Status] = @Status");
                                sqlParams.Add(new SqlParameter("@Status", intVal));
                            }
                            break;
                        }
                    case "Volume":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                sbWhereSql.Append(" AND [RefID] = @RefID");
                                sqlParams.Add(new SqlParameter("@RefID", intVal));
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            var wheresql = Conditions == null ? string.Empty : Conditions.ToQuerySql(sqlParams);

            if (!string.IsNullOrEmpty(wheresql))
            {
                sbWhereSql.Append(string.Format(" AND {0}", wheresql));
            }

            if (Param.PageSize > 0)
            {
                var sql = string.Format(@"

                SELECT @AllCount = COUNT(1) FROM [dbo].[ArchiveDoc_{0}_{1}_File] {4}

                SELECT TOP {2} *
                FROM 
                (

                 SELECT ROW_NUMBER() OVER(ORDER BY {5}) AS ROWNUMBER,* 
                 FROM [dbo].[ArchiveDoc_{0}_{1}_File] WITH(NOLOCK)
                 {4}
 
                ) A
                WHERE ROWNUMBER > {3}
                ORDER BY {5}", FondsNumber, ArchiveType, Param.PageSize, Param.PageSize * (Param.PageIndex - 1), sbWhereSql.ToString(), orderStr);

                var paramAllCount = new SqlParameter("@AllCount", 0) { Direction = ParameterDirection.Output };
                sqlParams.Add(paramAllCount);

                var context = new DMContext();
                var table = context.Database.SqlQueryForDataTatable(sql, sqlParams.ToArray());

                return new TableSource(table, Convert.ToInt32(paramAllCount.Value), Param.PageSize);
            }
            else
            {
                var sql = string.Format(@"
                SELECT  *
                FROM [dbo].[ArchiveDoc_{0}_{1}_File] WITH(NOLOCK)
                {2}
                ORDER BY {3}", FondsNumber, ArchiveType, sbWhereSql.ToString(), orderStr);

                var context = new DMContext();
                var table = context.Database.SqlQueryForDataTatable(sql, sqlParams.ToArray());
                return new TableSource(table, 0, 0);
            }
        }

        /// <summary>
        /// 下载整个案卷文件
        /// </summary>
        /// <param name="FondsNumber"></param>
        /// <param name="ArchiveType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DownloadFileActionResult DownloadArchiveFiles(string FondsNumber, string ArchiveType, int ID)
        {
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, FondsNumber, ArchiveType);
            var volumeFields = _IFieldService.GetFields(k, "Volume").Where(f => f.Main == true);

            var volName = "案卷" + ID;
            if (volumeFields.Count() > 0)
            {
                var volumeData = GetArchiveData(ID, FondsNumber, ArchiveType, EnumArchiveName.Volume);
                volName = string.Join("-", volumeFields.Select(f => volumeData[string.Format("_f{0}", f.ID)]));
            }

            k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, FondsNumber, ArchiveType);
            var fileFields = _IFieldService.GetFields(k, "File");
            var fileMainFields = fileFields.Where(f => f.Main == true);

            var param = new PageQueryParam();
            param.FilterCondtion = new Hashtable();
            param.FilterCondtion.Add("Volume", ID);

            var objAttachName = string.Format("ArchiveDoc_{0}_{1}_File", FondsNumber, ArchiveType);

            var files = GetArchiveFiles(FondsNumber, ArchiveType, param, null);

            var attachInfos = new List<SysAttachInfo>();

            foreach (DataRow dr in files.Source.Rows)
            {
                var fid = dr.Field<int>("ID");

                var fileName = "文件" + fid;

                if (fileMainFields.Count() > 0)
                {
                    fileName = string.Join("-", fileMainFields.Select(f => dr[string.Format("_f{0}", f.ID)]));
                }

                // 档案文件附件
                var attachs = _IObjectAttachService.GetAttachFiles(objAttachName, fid);

                foreach (var item in attachs)
                {
                    attachInfos.Add(new SysAttachInfo(item)
                    {
                        CustDirectory = string.Format("{0}\\{1}\\{2}", volName, fileName, item.Name)
                    });
                }
            }

            return new DownloadFileActionResult(attachInfos);
        }

        /// <summary>
        /// 创建档案
        /// </summary>
        public int CreateArchive(CreateArchiveInfo ArchiveInfo)
        {
            var archiveInfo = _IArchiveLibraryService.GetArchiveInfo(ArchiveInfo.FondsNumber, ArchiveInfo.ArchiveType, true);

            if (archiveInfo.HasProject && ArchiveInfo.ProjectID == 0)
            {
                ArchiveInfo.ProjectID = _createProject(ArchiveInfo);
            }

            var id = _createArchive(ArchiveInfo, archiveInfo);

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = id,
                Fonds = ArchiveInfo.FondsNumber,
                ArchiveType = ArchiveInfo.ArchiveType,
                LogDate = DateTime.Now,
                LogType = (int)ArchiveLogType.系统,
                LogUser = ArchiveInfo.UserID,
                LogContent = "自动组卷，创建档案"
            });

            return id;
        }

        private int _createProject(CreateArchiveInfo ArchiveInfo)
        {
            // 判断是否有项目字段信息
            if (ArchiveInfo.ProjectFields == null ||
                ArchiveInfo.ProjectFields.Count == 0 ||
                ArchiveInfo.ProjectFields[0].Value == null)
            {
                return 0;
            }

            var paramList = new List<SqlParameter>();

            paramList.Add(new SqlParameter("@FondsNumber", ArchiveInfo.FondsNumber));

            var sbSqlValues = new StringBuilder("@FondsNumber,");
            var sbSqlFields = new StringBuilder("[FondsNumber],");

            foreach (var field in ArchiveInfo.ProjectFields)
            {
                if (field.Value == null && field.NotNull)
                {
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);

                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
                else if (field.Value != null)
                {
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);

                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.Value));
                }
            }

            sbSqlValues.Append("@CreateUser");
            sbSqlFields.Append("[CreateUser]");
            paramList.Add(new SqlParameter("@CreateUser", ArchiveInfo.UserID));

            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var sql = string.Format(@"
                INSERT INTO [dbo].[Project]({0}) VALUES({1})
                SET @ID = @@IDENTITY ", sbSqlFields.ToString(), sbSqlValues.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);

            return id;
        }

        private int _createArchive(CreateArchiveInfo createArchiveInfo, ArchiveInfo archiveInfo)
        {
            var paramList = new List<SqlParameter>();

            paramList.Add(new SqlParameter("@FondsNumber", createArchiveInfo.FondsNumber));
            paramList.Add(new SqlParameter("@NodeID", createArchiveInfo.ArchiveNode));

            var sbSqlValues = new StringBuilder("@FondsNumber,@NodeID,");
            var sbSqlFields = new StringBuilder("[FondsNumber],[NodeID],");

            if (createArchiveInfo.ArchiveName == EnumArchiveName.File)
            {
                paramList.Add(new SqlParameter("@RefID", createArchiveInfo.ArchiveVolumeID));
                sbSqlValues.Append("@RefID,");
                sbSqlFields.Append("[RefID],");
            }
            else
            {
                paramList.Add(new SqlParameter("@AccessLevel", createArchiveInfo.AccessLevel));
                paramList.Add(new SqlParameter("@Copies", createArchiveInfo.Copies));
                sbSqlValues.Append("@AccessLevel,@Copies,");
                sbSqlFields.Append("[AccessLevel],[Copies],");

                if (archiveInfo.HasProject)
                {
                    paramList.Add(new SqlParameter("@ProjectID", createArchiveInfo.ProjectID));
                    sbSqlValues.Append("@ProjectID,");
                    sbSqlFields.Append("[ProjectID],");
                }

                if (archiveInfo.HasCategory)
                {
                    paramList.Add(new SqlParameter("@Category", createArchiveInfo.Category));
                    sbSqlValues.Append("@Category,");
                    sbSqlFields.Append("[Category],");
                }
            }

            var fields = archiveInfo.VolumeFields;

            switch (createArchiveInfo.ArchiveName)
            {
                case EnumArchiveName.Volume:
                    break;
                case EnumArchiveName.Box:
                    fields = archiveInfo.BoxFields;
                    break;
                case EnumArchiveName.File:
                    fields = archiveInfo.FileFields;
                    break;
                case EnumArchiveName.Project:
                    break;
                default:
                    break;
            }

            foreach (var field in fields)
            {
                var valueField = createArchiveInfo.Fields.FirstOrDefault(f => f.ID == field.ID);
                var value = valueField == null ? null : valueField.Value;

                if (value == null && field.NotNull)
                {
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);

                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
                else if (value != null)
                {
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);

                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), value));
                }
            }

            sbSqlValues.Append("@CreateUser");
            sbSqlFields.Append("[CreateUser]");
            paramList.Add(new SqlParameter("@CreateUser", createArchiveInfo.UserID));

            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var sql = string.Format(@"
                INSERT INTO [dbo].[ArchiveDoc_{0}_{1}_{2}]({3}) VALUES({4})
                SET @ID = @@IDENTITY ", createArchiveInfo.FondsNumber, createArchiveInfo.ArchiveType, createArchiveInfo.ArchiveName,
                sbSqlFields.ToString(), sbSqlValues.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);



            return id;
        }

        /// <summary>
        /// 更新档案
        /// </summary>
        /// 
        public void UpdateArchive(int ID, CreateArchiveInfo ArchiveInfo)
        {
            var archiveInfo = _IArchiveLibraryService.GetArchiveInfo(ArchiveInfo.FondsNumber, ArchiveInfo.ArchiveType, true);

            if (archiveInfo.HasProject && ArchiveInfo.ProjectID > 0)
            {
                _updateProject(ArchiveInfo);
            }
            else if (archiveInfo.HasProject)
            {
                ArchiveInfo.ProjectID = _createProject(ArchiveInfo);
            }

            _updateArchive(ID, ArchiveInfo, archiveInfo);

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = ID,
                Fonds = ArchiveInfo.FondsNumber,
                ArchiveType = ArchiveInfo.ArchiveType,
                LogDate = DateTime.Now,
                LogType = (int)ArchiveLogType.系统,
                LogUser = ArchiveInfo.UserID,
                LogContent = "档案数据修改"
            });
        }

        /// <summary>
        /// 更新案卷信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ArchiveInfo"></param>
        public void _updateArchive(int ID, CreateArchiveInfo createArchiveInfo, ArchiveInfo archiveInfo)
        {
            var paramList = new List<SqlParameter>();
            var sbSqlFields = new StringBuilder();

            var fields = archiveInfo.VolumeFields;

            switch (createArchiveInfo.ArchiveName)
            {
                case EnumArchiveName.Volume:
                    break;
                case EnumArchiveName.Box:
                    fields = archiveInfo.BoxFields;
                    break;
                case EnumArchiveName.File:
                    fields = archiveInfo.FileFields;
                    break;
                case EnumArchiveName.Project:
                    break;
                default:
                    break;
            }

            foreach (var field in createArchiveInfo.Fields)
            {
                var exitField = fields.FirstOrDefault(f => f.ID == field.ID);

                if (exitField != null)
                {
                    if (field.Value == null)
                    {
                        // 不为空的字段更新为默认值
                        if (field.NotNull)
                        {
                            sbSqlFields.AppendFormat("[_f{0}] = @_f{0},", field.ID);
                            paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                        }
                        else
                        {
                            sbSqlFields.AppendFormat("[_f{0}] = @_f{0},", field.ID);
                            paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), DBNull.Value));
                        }
                    }
                    else
                    {
                        sbSqlFields.AppendFormat("[_f{0}] = @_f{0},", field.ID);
                        paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.Value));
                    }
                }
            }

            if (createArchiveInfo.ArchiveName == EnumArchiveName.Volume || createArchiveInfo.ArchiveName == EnumArchiveName.Box)
            {
                if (archiveInfo.HasProject)
                {
                    sbSqlFields.Append("[ProjectID] = @ProjectID,");
                    paramList.Add(new SqlParameter("@ProjectID", createArchiveInfo.ProjectID));
                }

                if (archiveInfo.HasCategory)
                {
                    sbSqlFields.Append("[Category] = @Category,");
                    paramList.Add(new SqlParameter("@Category", createArchiveInfo.Category));
                }

                sbSqlFields.Append("[AccessLevel] = @AccessLevel,[Copies] = @Copies,");
                paramList.Add(new SqlParameter("@AccessLevel", createArchiveInfo.AccessLevel));
                paramList.Add(new SqlParameter("@Copies", createArchiveInfo.Copies));
            }

            sbSqlFields.Append("[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate");

            paramList.Add(new SqlParameter("@ModifyUser", createArchiveInfo.UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            paramList.Add(new SqlParameter("@ID", ID));

            var sql = string.Format(@"
                UPDATE [dbo].[ArchiveDoc_{0}_{1}_{2}] 
                SET {3}
                WHERE ID = @ID", createArchiveInfo.FondsNumber, createArchiveInfo.ArchiveType, createArchiveInfo.ArchiveName, sbSqlFields.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

        }

        /// <summary>
        /// 更新项目信息
        /// </summary>
        /// <param name="ArchiveInfo"></param>
        public void _updateProject(CreateArchiveInfo ArchiveInfo)
        {
            // 判断是否有项目字段信息
            if (ArchiveInfo.ProjectID == 0 ||
                ArchiveInfo.ProjectFields == null ||
                ArchiveInfo.ProjectFields.Count == 0 ||
                ArchiveInfo.ProjectFields[0].Value == null)
            {
                return;
            }

            var paramList = new List<SqlParameter>();
            var sbSqlFields = new StringBuilder();

            foreach (var field in ArchiveInfo.ProjectFields)
            {
                sbSqlFields.AppendFormat("[_f{0}] = @_f{0},", field.ID);
                paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.Value));
            }

            sbSqlFields.Append("[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate");

            paramList.Add(new SqlParameter("@ModifyUser", ArchiveInfo.UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            paramList.Add(new SqlParameter("@ID", ArchiveInfo.ProjectID));

            var sql = string.Format(@"
                UPDATE [dbo].[Project] 
                SET {0}
                WHERE ID = @ID", sbSqlFields.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

        }

        /// <summary>
        /// 删除档案
        /// </summary>
        public void DeleteArchive(string IDs, string FondsNumber, string ArchiveType, EnumArchiveName ArchiveName, string UserID)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@ModifyUser", UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            var sql = string.Format(@"
                UPDATE [dbo].[ArchiveDoc_{0}_{1}_{2}] 
                SET [IsDelete] = 1,[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate
                WHERE ID IN ({3})", FondsNumber, ArchiveType, ArchiveName, IDs.Trim(','));

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var idArray = IDs.Split(',');
            foreach (var id in idArray)
            {
                _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
                {
                    ArchiveID = int.Parse(id),
                    Fonds = FondsNumber,
                    ArchiveType = ArchiveType,
                    LogDate = DateTime.Now,
                    LogUser = int.Parse(UserID),
                    LogType = (int)ArchiveLogType.系统,
                    LogContent = "删除档案"
                });
            }
        }

        /// <summary>
        /// 案卷拆离
        /// </summary>
        public void RemoveArchiveFile(string IDs, string FondsNumber, string ArchiveType)
        {
            var context = new DMContext();

            var getFileSql = string.Format(@"SELECT [FileNumber],[FileID] FROM [dbo].[ArchiveDoc_{0}_{1}_File] WHERE ID IN ({2})", FondsNumber, ArchiveType, IDs.Trim(','));
            var files = context.Database.SqlQueryForDataTatable(getFileSql);

            foreach (DataRow dr in files.Rows)
            {
                var upSql = string.Format(@"UPDATE FileDoc_{0}_{1} SET [Status] = 1 WHERE ID = @ID", FondsNumber, dr.Field<int>("FileNumber"));
                context.Database.ExecuteSqlCommand(upSql, new SqlParameter("@ID", dr.Field<int>("FileID")));
            }

            var delSql = string.Format(@"DELETE [dbo].[ArchiveDoc_{0}_{1}_File] WHERE ID IN ({2})", FondsNumber, ArchiveType, IDs.Trim(','));
            context.Database.ExecuteSqlCommand(delSql);

            // 删除案卷文件附件
            foreach (var id in IDs.Split(','))
            {
                _IObjectAttachService.DeleteObjectAttach(int.Parse(id), string.Format("ArchiveDoc_{0}_{1}_File", FondsNumber, ArchiveType));


            }


        }

        /// <summary>
        /// 向案卷中追加文件
        /// </summary>
        public void AddArchiveFile(CreateArchiveInfo Info)
        {
            var context = new DMContext();

            var dtFiles = context.Database.SqlQueryForDataTatable(string.Format("SELECT * FROM [dbo].[FileDoc_{0}_{1}] WITH(NOLOCK) WHERE ID IN ({2})", Info.FondsNumber, Info.FileNumber, Info.FileIDs.Trim(',')));

            var fields = _IFieldService.GetFields(string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, Info.FondsNumber, Info.ArchiveType), "", true);

            // 将文件库内的文件设置为已归档
            _IFileDataService.SetFileArchived(Info.FondsNumber, Info.FileNumber, Info.FileIDs);

            foreach (DataRow fileRow in dtFiles.Rows)
            {
                insertArchiveFile(context, Info.FondsNumber, Info.FileNumber, Info.ArchiveType, Info.ArchiveNode, Info.ArchiveVolumeID, fileRow, fields, Info.UserID);
            }
        }

        /// <summary>
        /// 设置档案状态
        /// </summary>
        public void SetArchiveStatus(int ID, CreateArchiveInfo ArchiveInfo)
        {
            // 档案正式归档，创建搜索索引
            if (ArchiveInfo.ArchiveStatus == EnumArchiveStatus.正式)
            {
                CreateArchiveIndex(ID, ArchiveInfo.FondsNumber, ArchiveInfo.ArchiveType);
            }

            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@ModifyUser", ArchiveInfo.UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            paramList.Add(new SqlParameter("@ID", ID));
            paramList.Add(new SqlParameter("@Status", (int)ArchiveInfo.ArchiveStatus));

            var sql = string.Format(@"
                UPDATE [dbo].[ArchiveDoc_{0}_{1}_{2}] 
                SET [Status] = @Status,[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate
                WHERE ID = @ID", ArchiveInfo.FondsNumber, ArchiveInfo.ArchiveType, ArchiveInfo.ArchiveName);

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());


            var content = "";

            switch (ArchiveInfo.ArchiveStatus)
            {
                case EnumArchiveStatus.整编:
                    content = "状态变更：返回整编";
                    break;
                case EnumArchiveStatus.正式:
                    content = "状态变更：正式归档";
                    break;
                case EnumArchiveStatus.上架:
                    content = "状态变更：档案上架";
                    break;
                case EnumArchiveStatus.销毁:
                    content = "状态变更：档案销毁";
                    break;
                default:
                    break;
            }

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = ID,
                Fonds = ArchiveInfo.FondsNumber,
                ArchiveType = ArchiveInfo.ArchiveType,
                LogDate = DateTime.Now,
                LogUser = ArchiveInfo.UserID,

                LogType = (int)ArchiveLogType.系统,
                LogContent = content
            });
        }

        /// <summary>
        /// 创建档案索引
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="FondsNumber"></param>
        /// <param name="ArchiveType"></param>
        public void CreateArchiveIndex(int ID, string FondsNumber, string ArchiveType)
        {
            var indexDatas = new Dictionary<string, string>();

            indexDatas.Add("FondsNumber", FondsNumber);
            indexDatas.Add("ArchiveType", ArchiveType);

            var archiveTypeName = _IArchiveLibraryService.GetArchiveName(FondsNumber, ArchiveType);
            indexDatas.Add("ArchiveTypeName", archiveTypeName);

            #region 档案数据
            // 档案字段
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, FondsNumber, ArchiveType);
            var volumeFields = _IFieldService.GetFields(k, "Volume");

            // 档案数据
            var volumeData = GetArchiveData(ID, FondsNumber, ArchiveType, EnumArchiveName.Volume);
            var accessLevel = int.Parse(volumeData["AccessLevel"].ToString());

            foreach (var field in volumeFields)
            {
                // 允许字段被查询，则将该字段的内容进行索引
                if (field.ForSearch)
                {
                    var f = string.Format("_f{0}", field.ID);
                    indexDatas.Add(f + "_v", volumeData[f].ToString());
                }
            }
            #endregion

            #region 项目数据

            if (volumeData.ContainsKey("ProjectID"))
            {
                var projID = int.Parse(volumeData["ProjectID"].ToString());

                if (projID > 0)
                {
                    // 项目字段
                    k = string.Format("BusinessSystem.{0}.Field.Project", ConstValue.BusinessKey);
                    var projectFields = _IFieldService.GetFields(k, "Project");

                    var projData = GetProjectInfo(projID);

                    foreach (var field in projectFields)
                    {
                        // 允许字段被查询，则将该字段的内容进行索引
                        if (field.ForSearch)
                        {
                            var f = string.Format("_f{0}", field.ID);
                            indexDatas.Add(f + "_p", projData[f].ToString());
                        }
                    }
                }
            }

            #endregion

            #region 档案文件

            // 档案文件字段
            k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, FondsNumber, ArchiveType);
            var fileFields = _IFieldService.GetFields(k, "File");

            // 档案文件列表
            var param = new PageQueryParam();
            param.FilterCondtion = new Hashtable();
            param.FilterCondtion.Add("Volume", ID);

            var files = GetArchiveFiles(FondsNumber, ArchiveType, param, null);

            var hasFile = false;

            foreach (DataRow dr in files.Source.Rows)
            {
                var fid = dr.Field<int>("ID");

                // 档案文件信息
                var fileIndexData = new Dictionary<string, string>();

                fileIndexData.Add("FondsNumber", FondsNumber);
                fileIndexData.Add("ArchiveType", ArchiveType);
                fileIndexData.Add("ArchiveTypeName", archiveTypeName);
                fileIndexData.Add("ArchiveID", ID.ToString());

                foreach (var field in fileFields)
                {
                    // 允许字段被查询，则将该字段的内容进行索引
                    if (field.ForSearch)
                    {
                        var f = string.Format("_f{0}", field.ID);
                        fileIndexData.Add(f, dr[f].ToString());
                    }
                }

                // 档案文件附件
                var objAttachName = string.Format("ArchiveDoc_{0}_{1}_File", FondsNumber, ArchiveType);

                var attachIDs = _IObjectAttachService.GetAttachIDs(objAttachName, fid);

                if (attachIDs != null)
                {
                    hasFile = attachIDs.Count() > 0;

                    foreach (var aID in attachIDs)
                    {
                        _ILuceneIndexService.CreateIndexByFile(fid, accessLevel, aID, fileIndexData);
                    }
                }
            }

            #endregion

            indexDatas.Add("HasFile", hasFile ? "1" : "0");

            _ILuceneIndexService.CreateIndexByData(ID.ToString(), accessLevel, indexDatas);
        }

        public Dictionary<string, object> GetProjectInfo(int ID)
        {
            var sql = string.Format(@"
                SELECT  *
                FROM [dbo].[Project] WITH(NOLOCK)
                WHERE ID = {0}", ID);

            var context = new DMContext();
            var table = context.Database.SqlQueryForDataTatable(sql);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            var result = new Dictionary<string, object>();

            foreach (DataColumn col in table.Columns)
            {
                result.Add(col.ColumnName, table.Rows[0][col]);
            }

            return result;
        }

        public DataTable LoadProjectSource(string Filter)
        {
            var sql = new StringBuilder(@"
                SELECT  *
                FROM [dbo].[Project] WITH(NOLOCK)
                WHERE 1=1 ");

            var sqlParams = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(Filter))
            {
                sql.Append("AND ([_f1] LIKE @Filter OR [_f2] LIKE @Filter)");
                sqlParams.Add(new SqlParameter("@Filter", string.Format("%{0}%", Filter)));
            }

            var context = new DMContext();
            return context.Database.SqlQueryForDataTatable(sql.ToString(), sqlParams.ToArray());
        }

        /// <summary>
        /// 自动组卷
        /// </summary>
        private int AutoCreate(AutoCreateArchiveInfo CreateInfo, DataTable files)
        {
            var context = new DMContext();

            // 获取档案节点信息
            var nodeKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}", ConstValue.BusinessKey, CreateInfo.FondsNumber, CreateInfo.ArchiveNode);
            var nodeConfig = _IBaseConfig.GetConfig(nodeKey);
            var archiveType = nodeConfig.Tag.Trim();
            var nodeType = (ArchiveNodeType)Enum.Parse(typeof(ArchiveNodeType), nodeConfig.Type);

            // 获取档案类型信息

            var archiveTypeInfo = _IArchiveLibraryService.GetArchiveInfo(CreateInfo.FondsNumber, archiveType, true, false, true);

            //var archiveKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.", ConstValue.BusinessKey, CreateInfo.FondsNumber, archiveType);
            //var archiveTypeNodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(archiveKey), archiveKey);

            //var volume = archiveTypeNodes.FirstOrDefault(n => n.NodeName == "Volume");
            var refID = 0;

            if (archiveTypeInfo.HasVolume)
            {
                // 包含案卷
                //var fieldNode = volume.ChildNodes.FirstOrDefault(n => n.NodeName == "Field");
                //var volumeFiels = converToField(fieldNode.ChildNodes);

                // insert volume
                refID = insertArchiveVolume(context, CreateInfo, files, archiveTypeInfo);
            }
            else
            {
                //var boxNode = archiveTypeNodes.FirstOrDefault(n => n.NodeName == "Box");

                //// 盒
                //var fieldNode = boxNode.ChildNodes.FirstOrDefault(n => n.NodeName == "Field");
                //var boxFiels = converToField(fieldNode.ChildNodes);

                // insert BOX
                refID = insertArchiveBox(context, CreateInfo.FondsNumber, CreateInfo.FileNumber, archiveType, CreateInfo.ArchiveNode, files, archiveTypeInfo.BoxFields, CreateInfo.UserID);
            }

            // 不包含案卷，只有文件
            //var file = archiveTypeNodes.FirstOrDefault(n => n.NodeName == "File");
            //var fileFieldNode = file.ChildNodes.FirstOrDefault(n => n.NodeName == "Field");
            //var fileFiels = converToField(fileFieldNode.ChildNodes);

            foreach (DataRow fileRow in files.Rows)
            {
                insertArchiveFile(context, CreateInfo.FondsNumber, CreateInfo.FileNumber, archiveType, CreateInfo.ArchiveNode, refID, fileRow, archiveTypeInfo.FileFields, CreateInfo.UserID);
            }

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = refID,
                Fonds = CreateInfo.FondsNumber,
                ArchiveType = archiveType,
                LogDate = DateTime.Now,

                LogUser = CreateInfo.UserID,
                LogType = (int)ArchiveLogType.系统,
                LogContent = "自动组卷"
            });

            return refID;
        }

        /// <summary>
        /// 根据文件列表生成一个案卷
        /// </summary>
        private int insertArchiveVolume(DMContext context, AutoCreateArchiveInfo createInfo, DataTable files, ArchiveInfo archiveInfo)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@FondsNumber", archiveInfo.FondsNumber));
            paramList.Add(new SqlParameter("@NodeID", createInfo.ArchiveNode));
            paramList.Add(new SqlParameter("@FileNumber", createInfo.FileNumber));

            var sbSqlValues = new StringBuilder("@FondsNumber,@NodeID,@FileNumber,");
            var sbSqlFields = new StringBuilder("[FondsNumber],[NodeID],[FileNumber],");

            foreach (var field in archiveInfo.VolumeFields)
            {
                FieldMapping fieldMap = null;

                // 案卷文件表字段与文件库字段的映射
                if (field.Mappings != null)
                {
                    fieldMap = field.Mappings.FirstOrDefault(m => m.FileNumber == createInfo.FileNumber);
                }

                if (fieldMap != null)
                {
                    // 存在映射 
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);

                    // 根据文件库与档案文件的映射关系计算档案字段值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), getFileValueByMapping(field, fieldMap, files)));
                }
                else if (field.NotNull)
                {
                    // 不为空字段
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);

                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
            }

            sbSqlValues.Append("@CreateUser");
            sbSqlFields.Append("[CreateUser]");

            paramList.Add(new SqlParameter("@CreateUser", createInfo.UserID));
            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var sql = string.Format(@"
                INSERT INTO [dbo].[ArchiveDoc_{0}_{1}_Volume]({2}) VALUES({3})
                SET @ID = @@IDENTITY ", archiveInfo.FondsNumber, archiveInfo.Key, sbSqlFields.ToString(), sbSqlValues.ToString());

            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);

            return id;
        }

        /// <summary>
        /// 自动生成一个盒
        /// </summary>
        private int insertArchiveBox(DMContext context, string FondsNumber, string FileNumber, string archiveType, string NodeID, DataTable files, List<FieldInfo> fields, int UserID)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@FondsNumber", FondsNumber));
            paramList.Add(new SqlParameter("@NodeID", NodeID));
            paramList.Add(new SqlParameter("@FileNumber", FileNumber));

            var sbSqlValues = new StringBuilder("@FondsNumber,@NodeID,@FileNumber,");
            var sbSqlFields = new StringBuilder("[FondsNumber],[NodeID],[FileNumber],");

            foreach (var field in fields)
            {
                if (field.NotNull)
                {
                    // 不为空字段
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);

                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
            }

            sbSqlValues.Append("@CreateUser");
            sbSqlFields.Append("[CreateUser]");

            paramList.Add(new SqlParameter("@CreateUser", UserID));
            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var sql = string.Format(@"
                INSERT INTO [dbo].[ArchiveDoc_{0}_{1}_Box]({2}) VALUES({3})
                SET @ID = @@IDENTITY ", FondsNumber, archiveType, sbSqlFields.ToString(), sbSqlValues.ToString());

            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);

            return id;
        }


        /// <summary>
        /// 把文件转换到案卷文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="fields"></param>
        private int insertArchiveFile(DMContext context, string FondsNumber, string FileNumber, string archiveType, string NodeID, int volID, DataRow fileRow, List<FieldInfo> fields, int UserID)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@FondsNumber", FondsNumber));
            paramList.Add(new SqlParameter("@NodeID", NodeID));
            paramList.Add(new SqlParameter("@RefID", volID));
            paramList.Add(new SqlParameter("@FileNumber", FileNumber));
            paramList.Add(new SqlParameter("@FileID", fileRow.Field<int>("ID")));

            var sbSqlValues = new StringBuilder("@FondsNumber,@NodeID,@RefID,@FileNumber,@FileID,");
            var sbSqlFields = new StringBuilder("[FondsNumber],[NodeID],[RefID],[FileNumber],[FileID],");

            foreach (var field in fields)
            {
                // 案卷文件表字段与文件库字段的映射
                FieldMapping fieldMap = null;

                // 案卷文件表字段与文件库字段的映射
                if (field.Mappings != null)
                {
                    fieldMap = field.Mappings.FirstOrDefault(m => m.FileNumber == FileNumber);
                }

                if (fieldMap != null)
                {
                    // 存在映射 
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);

                    // 文件库与档案文件的映射关系只有一种 等于(因为不能是聚合关系)
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), fileRow["_f" + fieldMap.FileFieldID]));
                }
                else if (field.NotNull)
                {
                    // 不为空字段
                    sbSqlFields.AppendFormat("[_f{0}],", field.ID);
                    sbSqlValues.AppendFormat("@_f{0},", field.ID);

                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
            }

            sbSqlValues.Append("@CreateUser");
            sbSqlFields.Append("[CreateUser]");

            paramList.Add(new SqlParameter("@CreateUser", UserID));
            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var tableName = string.Format("ArchiveDoc_{0}_{1}_File", FondsNumber, archiveType);

            var sql = string.Format(@"
                INSERT INTO [dbo].[{0}]({1}) VALUES({2})
                SET @ID = @@IDENTITY ", tableName, sbSqlFields.ToString(), sbSqlValues.ToString());

            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);

            // 如果是自动组卷，则将原文件的附件拷贝一份到案卷文件下
            if (!string.IsNullOrEmpty(FileNumber))
            {
                var ids = _IObjectAttachService.GetAttachIDs(string.Format("FileDoc_{0}_{1}", FondsNumber, FileNumber), fileRow.Field<int>("ID")).ToList();

                foreach (var attachID in ids)
                {
                    _IObjectAttachService.Add(new ObjectAttachEntity()
                    {
                        AttachID = attachID,
                        ObjectKey = tableName,
                        ObjectID = id,
                    });
                }
            }


            return id;
        }

        /// <summary>
        /// 根据字段映射返回字段值
        /// </summary>
        /// <returns></returns>
        private object getFileValueByMapping(FieldInfo field, FieldMapping mapping, DataTable files)
        {
            var mapType = (FieldMappingTypeEnum)mapping.MappingType;
            var dataType = (FieldDataTypeEnum)field.DataType;
            switch (mapType)
            {
                case FieldMappingTypeEnum.等于:
                    // 取集合的第一行字段值
                    return files.Rows[0]["_f" + mapping.FileFieldID];
                case FieldMappingTypeEnum.求和:
                    if (dataType == FieldDataTypeEnum.整数 || dataType == FieldDataTypeEnum.小数)
                    {
                        return files.Compute(string.Format("Sum(_f{0})", mapping.FileFieldID), "true");
                    }
                    else if (field.NotNull)
                    {
                        return field.GetDefaultValue();
                    }
                    else
                    {
                        return DBNull.Value;
                    }
                case FieldMappingTypeEnum.平均:
                    if (dataType == FieldDataTypeEnum.整数 || dataType == FieldDataTypeEnum.小数)
                    {
                        return files.Compute(string.Format("AVG(_f{0})", mapping.FileFieldID), "true");
                    }
                    else if (field.NotNull)
                    {
                        return field.GetDefaultValue();
                    }
                    else
                    {
                        return DBNull.Value;
                    }
                case FieldMappingTypeEnum.最大:
                    return files.Compute(string.Format("MAX(_f{0})", mapping.FileFieldID), "true");
                case FieldMappingTypeEnum.最小:
                    return files.Compute(string.Format("MIN(_f{0})", mapping.FileFieldID), "true");
                default:
                    return DBNull.Value;
            }
        }

        /// <summary>
        /// 将配置节点转换为字段列表
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private List<FieldInfo> converToField(List<ConfigNode> nodes)
        {
            var result = new List<FieldInfo>();

            nodes.ForEach(n =>
            {
                var fieldInfo = new FieldInfo()
                {
                    ID = n.NodeName,
                    Name = n.NodeValue
                };

                var datatype = n.ChildNodes.SingleOrDefault(a => a.NodeName == "DataType");
                var length = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Length");
                var notnull = n.ChildNodes.SingleOrDefault(a => a.NodeName == "NotNull");
                var basedata = n.ChildNodes.SingleOrDefault(a => a.NodeName == "BaseData");

                fieldInfo.DataType = int.Parse(datatype.NodeValue);
                fieldInfo.Length = int.Parse(length.NodeValue);

                if (basedata != null)
                {
                    fieldInfo.BaseData = basedata.NodeValue;
                }
                if (notnull != null)
                {
                    fieldInfo.NotNull = true;
                }

                var map = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Mapping");
                if (map != null)
                {
                    fieldInfo.Mappings = new List<FieldMapping>();

                    foreach (var item in map.ChildNodes)
                    {
                        var f = item.ChildNodes.FirstOrDefault();

                        fieldInfo.Mappings.Add(new FieldMapping()
                        {
                            FileNumber = item.NodeName,
                            FileFieldID = f.NodeName,
                            MappingType = int.Parse(f.NodeValue)
                        });
                    }
                }

                result.Add(fieldInfo);
            });

            return result;
        }


    }
}
