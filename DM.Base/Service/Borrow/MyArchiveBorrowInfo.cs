using DM.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class MyArchiveBorrowInfo
    {
        public int BorrowID { get; set; }

        public string Name { get; set; }

        public string Fonds { get; set; }

        public string ArchiveType { get; set; }

        public int ArchiveID { get; set; }

        public string ArchiveTypeName { get; set; }

        public int Copies { get; set; }

        public int AccessLevel { get; set; }

        public int Status { get; set; }

        public int BorrowCount { get; set; }

        public bool IsTimeOut
        {
            get
            {
                return this.Status == (int)BorrowStatus.审核通过 && this.BorrowInfo != null && this.BorrowInfo.BackDate < DateTime.Now;
            }
        }

        public ArchiveBorrowEntity BorrowInfo { get; set; }

        public ArchiveInfo ArchiveInfo { get; set; }
    }
}
