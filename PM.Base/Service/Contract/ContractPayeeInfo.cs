using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// ContractPayee 扩展信息
    /// </summary>
    public partial class ContractPayeeInfo
    {
        public ContractPayeeInfo()
        {
        }

        public ContractPayeeInfo(ContractPayeeEntity Entity)
        {
            this.ID = Entity.ID;
            this.ContractObjectID = Entity.ContractObjectID;
            this.Fee = Entity.Fee;
            this.Date = Entity.Date;
            this.Note = Entity.Note;
            this.InvoiceType = Entity.InvoiceType;
            this.IsDelete = Entity.IsDelete;
            this.Type = Entity.Type;
        }

        public Int32 ID { get; set; }
        public Int32 ContractObjectID { get; set; }
        public Decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public String Note { get; set; }
        public Int32 InvoiceType { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 Type { get; set; }

        public String ObjectKey { get; set; }

        public Int32 ObjectID { get; set; }

        public List<int> AttachIDs { get; set; }


    }
}
