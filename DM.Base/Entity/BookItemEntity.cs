using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class BookItemEntity
    {
        public int ID { get; set; }

        public int BookID { get; set; }

        public int Status { get; set; }

        public string BarCode { get; set; }

        public int? BorrowUser { get; set; }

        public DateTime? BorrowOutDate { get; set; }

        public DateTime? BackDate { get; set; }

        public DateTime? DestroyDate { get; set; }

    }
}
