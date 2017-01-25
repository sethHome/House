using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// Bid 扩展信息
    /// </summary>
    public partial class BidInfo
    {
        public BidInfo()
        {
        }

        public BidInfo(BidEntity Entity)
        {
            this.ID = Entity.ID;
            this.Name = Entity.Name;
            this.Manager = Entity.Manager;
            this.CustomerID = Entity.CustomerID;
            this.PersonID = Entity.PersonID;
            this.Agency = Entity.Agency;
            this.BidStatus = Entity.BidStatus;
            this.ServiceFee = Entity.ServiceFee;
            this.BidFee = Entity.BidFee;
            this.DepositFee = Entity.DepositFee;
            this.DepositFeeStatus = Entity.DepositFeeStatus;
            this.LimitFee = Entity.LimitFee;
            this.IsTentative = Entity.IsTentative;
            this.BidDate = Entity.BidDate;
            this.SuccessfulBidDate = Entity.SuccessfulBidDate;
            this.Note = Entity.Note;
            this.IsDelete = Entity.IsDelete;
        }

        public Int32 ID { get; set; }
        public String Name { get; set; }
        public Int32 Manager { get; set; }
        public Int32 CustomerID { get; set; }
        public Int32 PersonID { get; set; }
        public String Agency { get; set; }
        public Int32 BidStatus { get; set; }
        public Decimal ServiceFee { get; set; }
        public Decimal BidFee { get; set; }
        public Decimal DepositFee { get; set; }
        public Int32 DepositFeeStatus { get; set; }
        public Decimal LimitFee { get; set; }
        public Boolean IsTentative { get; set; }
        public DateTime BidDate { get; set; }
        public DateTime? SuccessfulBidDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

        public CustomerEntity Customer { get; set; }

        public CustomerPersonEntity Person { get; set; }

    }
}
