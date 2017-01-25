using DM.Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveBorrowEntity
    {
        public int ID { get; set; }

        public int BorrowDept { get; set; }

        public int BorrowUser { get; set; }

        public DateTime BackDate { get; set; }

        public string Note { get; set; }

        public DateTime BorrowDate { get; set; }

        public int Status { get; set; }

        public ArchiveBorrowEntity()
        {
         
        }

        public ArchiveBorrowEntity(BorrowInfo Info)
        {
            this.ID = Info.ID;
            this.BorrowDept = Info.BorrowDept;
            this.BorrowUser = Info.BorrowUser;
            this.BackDate = Info.BackDate;
            this.Note = Info.Note;
            this.BorrowDate = Info.BorrowDate;
            this.Status = Info.Status;
        }
    }
}
