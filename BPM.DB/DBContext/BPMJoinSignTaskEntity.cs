using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.DB
{
   public class BPMJoinSignTaskEntity
    {
        public Int32 ID { get; set; }
        public Guid TaskID { get; set; }
        public Int32 UserID { get; set; }
        public Int32 Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public Boolean Result { get; set; }
        public Boolean IsChecked { get; set; }
    }
}
