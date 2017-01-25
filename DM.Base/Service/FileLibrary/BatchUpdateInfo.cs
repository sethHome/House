using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class BatchUpdateInfo
    {
        public string NodeID { get; set; }

        public string Dept { get; set; }

        // 更新字段
        public FieldInfo Field { get; set; }

        // 更新条件
        public List<Condition> Conditions { get; set; }

        public string UpdateIDs { get; set; }

        // 更新值
        public List<ValueExpression> Expressions { get; set; }

        // 字段更新区域
        public int PartStart { get; set; }
        public int PartLength { get; set; }

        // 字段替换内容
        public string ReplaceValue { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        public int UpdateUser { get; set; }
    }

    public class ValueExpression
    {
        public FieldInfo Field { get; set; }

        public object Value { get; set; }

        public int FieldStart { get; set; }

        public int FieldLength { get; set; }

        public int IdentityStart { get; set; }

        public int IdentityLength { get; set; }

        public string IdentityFill { get; set; }
    }
}
