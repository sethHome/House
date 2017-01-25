using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArchiveQueryInfo
    {
        public string ConditionsSqlStr { get; set; }

        public List<Condition> Conditions  { get; set; }
    }
}
