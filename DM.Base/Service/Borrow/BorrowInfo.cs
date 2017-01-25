using DM.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class BorrowInfo
    {
        public int ID { get; set; }

        public int BorrowDept { get; set; }

        public int BorrowUser { get; set; }

        public DateTime BackDate { get; set; }

        public string Note { get; set; }

        public DateTime BorrowDate { get; set; }

        public int Status { get; set; }

        public Dictionary<string, object> FlowData { get; set; }

        public List<ArchiveBorrowItemEntity> Items { get; set; }
    }
}
