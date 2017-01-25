using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArchiveNodeInfo
    {
        public string FondsNumber { get; set; }

        public string ParentFullKey { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public ArchiveNodeType NodeType { get; set; }

        public string ArchiveType { get; set; }

        public string Note { get; set; }

        public string ConditionsSqlStr { get; set; }

        public bool Disabled { get; set; }

        public bool HasVolume { get; set; }

        public bool HasProject { get; set; }

        public bool HasCategory { get; set; }

        public List<Condition> Conditions { get; set; }

        public List<ArchiveNodeInfo> Children { get; set; }
    }
}
