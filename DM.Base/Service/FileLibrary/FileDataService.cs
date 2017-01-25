using Api.Framework.Core;
using Api.Framework.Core.Attach;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Office;
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
    public class FileDataService : IFileDataService
    {
        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        public int AddFieldData(string FondsNumber, string FileNumber, int UserID, FileDataInfo Info, string NodeID = "",string dept = "")
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@FondsNumber", FondsNumber));
            paramList.Add(new SqlParameter("@NodeID", NodeID));

            var sbSql = new StringBuilder("@FondsNumber,@NodeID,");
            var sbSqlFields = new StringBuilder("[FondsNumber],[NodeID],");

            foreach (var field in Info.Fields)
            {
                sbSql.AppendFormat("@_f{0},", field.ID);
                sbSqlFields.AppendFormat("[_f{0}],", field.ID);

                if (field.NotNull && field.Value == null)
                {
                    // 默认值
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.GetDefaultValue()));
                }
                else
                {
                    paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.Value));
                }
            }
            sbSql.Append("@Dept,@CreateUser");
            sbSqlFields.Append("[Dept],[CreateUser]");

            paramList.Add(new SqlParameter("@Dept", dept));
            paramList.Add(new SqlParameter("@CreateUser", UserID));
            paramList.Add(new SqlParameter("@ID", 0) { Direction = System.Data.ParameterDirection.Output });

            var sql = string.Format(@"
                INSERT INTO [dbo].[FileDoc_{0}_{1}]({2}) VALUES({3})
                SET @ID = @@IDENTITY ", FondsNumber, FileNumber, sbSqlFields.ToString(), sbSql.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

            var id = Convert.ToInt32(paramList.Last().Value);
            foreach (var attachID in Info.AttachIDs)
            {
                _IObjectAttachService.Add(new ObjectAttachEntity()
                {
                    ObjectKey = string.Format("FileDoc_{0}_{1}", FondsNumber, FileNumber),
                    ObjectID = id,
                    AttachID = attachID
                });
            }

            return id;
        }

        public TableSource GetFileData(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions)
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
                            if (val != "0")
                            {
                                sbWhereSql.AppendFormat(" AND [ID] IN ({0})", val);
                            }

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
                    case "Dept":
                        {
                            if (val != "0")
                            {
                                sbWhereSql.Append(" AND [Dept] = @Dept");
                                sqlParams.Add(new SqlParameter("@Dept", val));
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

                SELECT @AllCount = COUNT(1) FROM [dbo].[FileDoc_{0}_{1}] {4}

                SELECT TOP {2} *
                FROM 
                (

                 SELECT ROW_NUMBER() OVER(ORDER BY {5}) AS ROWNUMBER,* 
                 FROM [dbo].[FileDoc_{0}_{1}] WITH(NOLOCK)
                 {4}
 
                ) A
                WHERE ROWNUMBER > {3}
                ORDER BY {5}", FondsNumber, FileNumber, Param.PageSize, Param.PageSize * (Param.PageIndex - 1), sbWhereSql.ToString(), orderStr);

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
                FROM [dbo].[FileDoc_{0}_{1}] WITH(NOLOCK)
                {2}
                ORDER BY {3}", FondsNumber, FileNumber, sbWhereSql.ToString(), orderStr);

                var context = new DMContext();
                var table = context.Database.SqlQueryForDataTatable(sql, sqlParams.ToArray());
                return new TableSource(table, 0, 0);
            }
        }

        public DataTable GetExportData(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions,out Dictionary<string, ColumnMapInfo> mapInfos)
        {
            var source = this.GetFileData(FondsNumber, FileNumber, Param, Conditions);

            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.", ConstValue.BusinessKey, FondsNumber, FileNumber);
            var fields = _IFieldService.GetFields(k, "");

            var removeColumns = new List<DataColumn>();
            mapInfos = new Dictionary<string, ColumnMapInfo>();
            var baseDataDic = new Dictionary<string, Dictionary<string, string>>();

            foreach (DataColumn column in source.Source.Columns)
            {
                var field = fields.FirstOrDefault(f => "_f" + f.ID == column.ColumnName);

                var mapInfo = new ColumnMapInfo();

                if (field != null)
                {
                    mapInfo.HeadText = field.Name;

                    if (field.DataType == (int)FieldDataTypeEnum.基础数据)
                    {
                        mapInfo.HasBaseData = true;
                        mapInfo.Values = _IEnum.GetEnumDic(ConstValue.BusinessKey, field.BaseData);
                    }

                    mapInfos.Add(column.ColumnName,mapInfo);
                }

                else if (column.ColumnName == "FondsNumber")
                {
                    mapInfo.HeadText = "全宗号";
                    mapInfos.Add(column.ColumnName, mapInfo);
                }
                else if (column.ColumnName == "Status")
                {
                    mapInfo.HeadText = "状态";
                    mapInfo.HasBaseData = true;
                    mapInfo.Values = _IEnum.GetEnumDic(ConstValue.BusinessKey, "FileStatus");
                    mapInfos.Add(column.ColumnName, mapInfo);
                }
                else
                {
                    removeColumns.Add(column);
                }
            }

            foreach (var item in removeColumns)
            {
                source.Source.Columns.Remove(item);
            }

            return source.Source;
        }

        public void UpdateFieldData(int ID, string FondsNumber, string FileNumber, int UserID, List<FieldInfo> Fields)
        {
            var paramList = new List<SqlParameter>();
            var sbSqlFields = new StringBuilder();

            foreach (var field in Fields)
            {
                sbSqlFields.AppendFormat("[_f{0}] = @_f{0},", field.ID);
                paramList.Add(new SqlParameter(string.Format("@_f{0}", field.ID), field.Value));
            }

            sbSqlFields.Append("[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate");

            paramList.Add(new SqlParameter("@ModifyUser", UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            paramList.Add(new SqlParameter("@ID", ID));

            var sql = string.Format(@"
                UPDATE [dbo].[FileDoc_{0}_{1}] 
                SET {2}
                WHERE ID = @ID", FondsNumber, FileNumber, sbSqlFields.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());

        }

        public void SetFileArchived(string FondsNumber, string FileNumber, string Ids)
        {
            var sql = string.Format(@"
                UPDATE [dbo].[FileDoc_{0}_{1}]
                SET [Status] = 2
                WHERE ID IN ({2})", FondsNumber, FileNumber, Ids.Trim(','));

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql);
        }

        public void SetFileArchived(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions)
        {
            var sbWhereSql = new StringBuilder("WHERE [IsDelete] = 0");
            var sqlParams = new List<SqlParameter>();

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
                            if (val != "0")
                            {
                                sbWhereSql.AppendFormat(" AND [ID] IN ({0})", val);
                            }

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
                    case "Dept":
                        {
                            if (val != "0")
                            {
                                sbWhereSql.Append(" AND [Dept] = @Dept");
                                sqlParams.Add(new SqlParameter("@Dept", val));
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

            var sql = string.Format(@"
                UPDATE [dbo].[FileDoc_{0}_{1}]
                SET [Status] = 2
                {2}", FondsNumber, FileNumber, sbWhereSql.ToString());

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, sqlParams.ToArray());
        }

        public void DeleteFieldData(string IDs, string FondsNumber, string FileNumber, int UserID)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@ModifyUser", UserID));
            paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));
            var sql = string.Format(@"
                UPDATE [dbo].[FileDoc_{0}_{1}] 
                SET [IsDelete] = 1,[ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate
                WHERE ID IN ({2})", FondsNumber, FileNumber, IDs.Trim(','));

            var context = new DMContext();
            context.Database.ExecuteSqlCommand(sql, paramList.ToArray());
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="FondsNumber"></param>
        /// <param name="FileNumber"></param>
        /// <param name="Info"></param>
        public void BatchUpdate(string FondsNumber, string FileNumber, BatchUpdateInfo Info)
        {
            if (Info.Field == null || string.IsNullOrEmpty(Info.Field.ID))
            {
                throw new Exception("更新字段不能为空");
            }

            var paramList = new List<SqlParameter>();

            if (Info.Field.DataType == (int)FieldDataTypeEnum.字符)
            {
                var valueSB = new StringBuilder();

                var index = 1;
                var expressions = new StringBuilder();
                var identityExp = new StringBuilder();

                foreach (var v in Info.Expressions)
                {
                    if (v.Value != null)
                    {
                        // 字符串
                        paramList.Add(new SqlParameter("@Value" + index, v.Value));
                        expressions.AppendFormat("@Value{0} + ", index);
                    }
                    else if (v.Field != null && !string.IsNullOrEmpty(v.Field.ID))
                    {
                        // 字段
                        if (v.Field.DataType == (int)FieldDataTypeEnum.字符)
                        {
                            expressions.AppendFormat("[_f{0}] + ", v.Field.ID);
                        }
                        else if (v.Field.DataType == (int)FieldDataTypeEnum.日期)
                        {
                            expressions.AppendFormat("CONVERT(NVARCHAR(100),[_f{0}],111) + ", v.Field.ID);
                        }
                        else
                        {
                            expressions.AppendFormat("CONVERT(NVARCHAR(100),[_f{0}],0) + ", v.Field.ID);
                        }
                    }
                    else if (v.IdentityLength > 0)
                    {
                        // 自增加编号

                        identityExp.AppendFormat("@Identity{0} = @Identity{0} + 1 , ", index);

                        expressions.AppendFormat("CONVERT(NVARCHAR({1}), dbo.PadLeft(@Identity{0}, '{2}', {1}),0) + ", index, v.IdentityLength, v.IdentityFill);

                        paramList.Add(new SqlParameter("@Identity" + index, v.IdentityStart));
                    }

                    index++;
                }

                var expressionsStr = expressions.ToString().Trim().TrimEnd('+');

                // 节点条件
                var sbWhereSql = new StringBuilder("WHERE NodeID = @NodeID");
                paramList.Add(new SqlParameter("@NodeID", Info.NodeID));

                // 部门条件
                if (!string.IsNullOrEmpty(Info.Dept) && Info.Dept != "0")
                {
                    sbWhereSql.Append(" AND Dept = @Dept ");
                    paramList.Add(new SqlParameter("@Dept", Info.Dept));
                }

                if (!string.IsNullOrEmpty(Info.UpdateIDs))
                {
                    sbWhereSql.AppendFormat(" AND [ID] IN ({0})", Info.UpdateIDs.Trim(','));
                }
                else if (Info.Conditions != null)
                {
                    var wheresql = Info.Conditions.ToQuerySql(paramList);
                    if (!string.IsNullOrEmpty(wheresql))
                    {
                        sbWhereSql.AppendFormat(" ADN {0}", wheresql);
                    }
                }

                var sql = "";
                if (!string.IsNullOrEmpty(Info.ReplaceValue))
                {
                    // 字段的查找替换
                    sql = string.Format(@"
                    UPDATE [dbo].[FileDoc_{0}_{1}] 
                    SET {4} [ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate, [_f{2}] = REPLACE([_f{2}],@ReplaceValue,{3})
                    {5}", FondsNumber, FileNumber, Info.Field.ID, expressionsStr, identityExp, sbWhereSql);

                    paramList.Add(new SqlParameter("@ReplaceValue", Info.ReplaceValue));
                }
                else if (Info.PartLength > 0)
                {
                    // 字段的区域更新
                    sql = string.Format(@"
                    UPDATE [dbo].[FileDoc_{0}_{1}] 
                    SET {4} [ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate, [_f{2}] = dbo.ReplacePart([_f{2}],@PartStart,@PartLength,{3})
                    {5}", FondsNumber, FileNumber, Info.Field.ID, expressionsStr, identityExp, sbWhereSql);

                    paramList.Add(new SqlParameter("@PartStart", Info.PartStart));
                    paramList.Add(new SqlParameter("@PartLength", Info.PartLength));
                }
                else
                {
                    sql = string.Format(@"
                    UPDATE [dbo].[FileDoc_{0}_{1}] 
                    SET {4} [ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate, [_f{2}] = {3}
                    {5}", FondsNumber, FileNumber, Info.Field.ID, expressionsStr, identityExp, sbWhereSql);
                }

                paramList.Add(new SqlParameter("@ModifyUser", Info.UpdateUser));
                paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));

                var context = new DMContext();
                context.Database.ExecuteSqlCommand(sql, paramList.ToArray());
            }
            else
            {
                paramList.Add(new SqlParameter("@Value", Info.Expressions.First().Value));

                // 节点条件
                var sbWhereSql = new StringBuilder("WHERE NodeID = @NodeID");
                paramList.Add(new SqlParameter("@NodeID", Info.NodeID));

                // 部门条件
                if (!string.IsNullOrEmpty(Info.Dept) && Info.Dept != "0")
                {
                    sbWhereSql.Append(" AND Dept = @Dept ");
                    paramList.Add(new SqlParameter("@Dept", Info.Dept));
                }

                if (!string.IsNullOrEmpty(Info.UpdateIDs))
                {
                    sbWhereSql.AppendFormat(" AND [ID] IN ({0})", Info.UpdateIDs.Trim(','));
                }
                else if (Info.Conditions != null)
                {
                    var wheresql = Info.Conditions.ToQuerySql(paramList);
                    if (!string.IsNullOrEmpty(wheresql))
                    {
                        sbWhereSql.AppendFormat(" ADN {0}", wheresql);
                    }
                }

                var sql = string.Format(@"
                UPDATE [dbo].[FileDoc_{0}_{1}] 
                SET [ModifyUser] = @ModifyUser,[ModifyDate] = @ModifyDate,[_f{2}] = @Value
                {3}", FondsNumber, FileNumber, Info.Field.ID, sbWhereSql);

                paramList.Add(new SqlParameter("@ModifyUser", Info.UpdateUser));
                paramList.Add(new SqlParameter("@ModifyDate", DateTime.Now));

                var context = new DMContext();
                context.Database.ExecuteSqlCommand(sql, paramList.ToArray());
            }
        }

    }
}
