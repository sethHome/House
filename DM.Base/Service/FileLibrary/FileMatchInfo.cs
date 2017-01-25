using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class FileMatchInfo
    {
        public string FondsNumber { get; set; }

        public string FileNumber { get; set; }

        public string NodeID { get; set; }

        // 条件
        public List<Condition> Conditions { get; set; }

        public List<ValueExpression> Expressions { get; set; }

        public List<string> FileNames { get; set; }
    }
}
