using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveBorrowItemEntity
    {
        public int ID { get; set; }

        public int BorrowID { get; set; }

        public int ArchiveID { get; set; }

        public string Fonds { get; set; }

        public string ArchiveType { get; set; }

        public int Count { get; set; }

        public int Status { get; set; }
    }
}
