using DM.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class BookInfo
    {
        public int ItemID { get; set; }

        public int BookID { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public int Press { get; set; }

        public int Count { get; set; }

        public int Type { get; set; }

        public decimal Price { get; set; }

        public int Year { get; set; }

        public string Author { get; set; }

        public string Style { get; set; }

        public string Category { get; set; }

        public string Note { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? LastModifyDate { get; set; }

        public int? LastModifyUser { get; set; }

        public int Status { get; set; }

        public string BarCode { get; set; }

        public int? BorrowUser { get; set; }

        public DateTime? BorrowOutDate { get; set; }

        public DateTime? BackDate { get; set; }

        public DateTime? DestroyDate { get; set; }
    }
}
