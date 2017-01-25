using DM.Base.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base
{
    public static class ConstValue
    {
        public const string BusinessKey = "System2";

        public static string ToQuerySql(this List<Condition> Conditions, List<SqlParameter> Params)
        {
            if (Conditions == null)
            {
                return string.Empty;
            }

            var sqlBulder = new StringBuilder();

            var index = 0;

            foreach (var item in Conditions)
            {
                if (item.Operator.ToUpper() != "IS NULL" && item.Operator.ToUpper() != "IS NOT NULL" &&
                    item.Value == null || string.IsNullOrEmpty(item.Value.ToString()))
                {
                    continue;
                }

                var paramName = string.Format("@_f{0}", item.Field.ID);

                if (Params.Exists(p => p.ParameterName == paramName))
                {
                    var pCount = Params.Count(p => p.ParameterName.StartsWith(paramName+"_")) + 1;
                    paramName = string.Format("@_f{0}_{1}", item.Field.ID, pCount);
                }
                

                Params.Add(new SqlParameter(paramName, item.Value));

                if (item.Operator == "LIKE" || item.Operator == "NOT LIKE")
                {
                    paramName = string.Format("'%' + {0} + '%'", paramName);
                }

                sqlBulder.AppendFormat(" {0} _f{1} {2} {3}", index == 0 ? "" : item.LogicOperation, item.Field.ID, item.Operator, paramName);

                index++;
            }

            return sqlBulder.ToString();
        }

        public static string ToQuerySql(this List<Condition> Conditions)
        {
            var sqlBulder = new StringBuilder();
            var index = 0;

            foreach (var item in Conditions)
            {
                var paramValue = item.Value;

                if (item.Operator == "LIKE" || item.Operator == "NOT LIKE")
                {
                    paramValue = string.Format("%{0}%", paramValue);
                }

                if (item.Field.DataType == 1 || item.Field.DataType == 4)
                {
                    // 整数
                    sqlBulder.AppendFormat(" {0} _f{1} {2} {3}", index == 0 ? "" : item.LogicOperation, item.Field.ID, item.Operator, paramValue);
                }
                else
                {
                    // 字符、日期
                    sqlBulder.AppendFormat(" {0} _f{1} {2} '{3}'", index == 0 ? "" : item.LogicOperation, item.Field.ID, item.Operator, paramValue);
                }

                index++;
            }

            return sqlBulder.ToString();
        }

       
    }
}
