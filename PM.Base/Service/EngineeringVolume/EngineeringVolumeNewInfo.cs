using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class EngineeringVolumeNewInfo
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public String Number { get; set; }
        public String Name { get; set; }
        public Int32 Designer { get; set; }
        public Int32 Checker { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public String Note { get; set; }
        public bool IsModified { get; set; }

        public List<TaskInfo> TaskUsers { get; set; }
    }
}
