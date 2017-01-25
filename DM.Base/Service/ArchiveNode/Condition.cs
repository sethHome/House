using Api.Framework.Core.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class Condition
    {
        public FieldInfo Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public string LogicOperation { get; set; }
    }
}
