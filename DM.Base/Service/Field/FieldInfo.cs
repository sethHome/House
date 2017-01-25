using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class FieldInfo
    {
        public string Fonds { get; set; }
        public string Archive { get; set; }
        public string ParentKey { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public int DataType { get; set; }

        public string BaseData { get; set; }

        public int Length { get; set; }

        public bool NotNull { get; set; }

        public bool ForSearch { get; set; }

        public bool Main { get; set; }

        public object Value { get; set; }

        public string Default { get; set; }

        public List<FieldMapping> Mappings { get; set; }

        public int Index { get; set; }

        // 根据字段默认值
        public object GetDefaultValue()
        {
            var fieldType = (FieldDataTypeEnum)this.DataType;

            if (string.IsNullOrEmpty(this.Default))
            {
                switch (fieldType)
                {
                    case FieldDataTypeEnum.整数:
                        return 0;
                    case FieldDataTypeEnum.字符:
                        return string.Empty;
                    case FieldDataTypeEnum.日期:
                        return DateTime.Parse("2000/01/01");
                    case FieldDataTypeEnum.基础数据:
                        return 0;
                    case FieldDataTypeEnum.小数:
                        return 0.0d;
                    default:
                        return string.Empty ;
                }
            }
            else
            {
                switch (fieldType)
                {
                    case FieldDataTypeEnum.整数:
                        return int.Parse(this.Default);
                    case FieldDataTypeEnum.字符:
                        return this.Default;
                    case FieldDataTypeEnum.日期:
                        return DateTime.Parse(this.Default);
                    case FieldDataTypeEnum.基础数据:
                        return int.Parse(this.Default);
                    case FieldDataTypeEnum.小数:
                        return double.Parse(this.Default);
                    default:
                        return this.Default;
                }
            }
        }
    }
}
