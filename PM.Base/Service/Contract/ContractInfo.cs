using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// Contract 扩展信息
    /// </summary>
    public partial class ContractInfo
    {
        public ContractInfo()
        {
        }

        public ContractInfo(ContractEntity Entity)
        {
            this.ID = Entity.ID;
            this.Number = Entity.Number;
            this.Name = Entity.Name;
            this.CustomerID = Entity.CustomerID;
            this.SignDate = Entity.SignDate;
            this.Fee = Entity.Fee;
            this.Type = Entity.Type;
            this.Status = Entity.Status;
            this.Note = Entity.Note;
            this.CreateDate = Entity.CreateDate;
            this.IsDelete = Entity.IsDelete;
        }

        public Int32 ID { get; set; }
        public String Number { get; set; }
        public String Name { get; set; }
        public Int32 CustomerID { get; set; }
        public DateTime SignDate { get; set; }
        public Decimal Fee { get; set; }
        public Int32 Type { get; set; }
        public Int32 Status { get; set; }
        public String Note { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

        public List<EngineeringEntity> Engineerings { get; set; }

        public CustomerEntity Customer { get; set; }

        /// <summary>
        /// 已收款
        /// </summary>
        public decimal? PayeeFee { get; set; }

        /// <summary>
        /// 已开票
        /// </summary>
        public decimal? PayeeInvoice { get; set; }
    }
}
