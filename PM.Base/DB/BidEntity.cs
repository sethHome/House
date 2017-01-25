using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Bid 
    /// </summary>
    public partial class BidEntity
    {
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


        public BidEntity()
        {
        }

        public BidEntity(BidInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Manager = Info.Manager;
            this.CustomerID = Info.CustomerID;
            this.PersonID = Info.PersonID;
            this.Agency = Info.Agency;
            this.BidStatus = Info.BidStatus;
            this.ServiceFee = Info.ServiceFee;
            this.BidFee = Info.BidFee;
            this.DepositFee = Info.DepositFee;
            this.DepositFeeStatus = Info.DepositFeeStatus;
            this.LimitFee = Info.LimitFee;
            this.IsTentative = Info.IsTentative;
            this.BidDate = Info.BidDate;
            this.SuccessfulBidDate = Info.SuccessfulBidDate;
            this.Note = Info.Note;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(BidEntity Entity)
        {
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
        }
    }
}
