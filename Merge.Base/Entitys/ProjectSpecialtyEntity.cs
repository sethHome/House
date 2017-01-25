using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Entitys
{
    public class ProjectSpecialtyEntity
    {
        public int ID { get; set; }

        public int ProjectID { get; set; }

        public int SpecilID { get; set; }

        public int Manager { get; set; }

        public string Note { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public bool IsMerge { get; set; }

        public bool IsFinish { get; set; }
    }
}
