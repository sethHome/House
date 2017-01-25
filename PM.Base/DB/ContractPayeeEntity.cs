using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-ContractPayee 
    /// </summary>
    public partial class ContractPayeeEntity
    {
        public Int32 ID { get; set; }
        public Int32 ContractObjectID { get; set; }
        public Decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public String Note { get; set; }
        public Int32 InvoiceType { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 Type { get; set; }


        public ContractPayeeEntity()
        {
        }

        public ContractPayeeEntity(ContractPayeeInfo Info)
        {
            this.ID = Info.ID;
            this.ContractObjectID = Info.ContractObjectID;
            this.Fee = Info.Fee;
            this.Date = Info.Date;
            this.Note = Info.Note;
            this.InvoiceType = Info.InvoiceType;
            this.IsDelete = Info.IsDelete;
            this.Type = Info.Type;
        }

        public void SetEntity(ContractPayeeEntity Entity)
        {
            if (!this.ContractObjectID.Equals(Entity.ContractObjectID))
            {
                this.ContractObjectID = Entity.ContractObjectID;
            }
            if (!this.Fee.Equals(Entity.Fee))
            {
                this.Fee = Entity.Fee;
            }
            if (!this.Date.Equals(Entity.Date))
            {
                this.Date = Entity.Date;
            }
            if (!this.Note.Equals(Entity.Note))
            {
                this.Note = Entity.Note;
            }
            if (!this.InvoiceType.Equals(Entity.InvoiceType))
            {
                this.InvoiceType = Entity.InvoiceType;
            }
            if (!this.IsDelete.Equals(Entity.IsDelete))
            {
                this.IsDelete = Entity.IsDelete;
            }
            if (!this.Type.Equals(Entity.Type))
            {
                this.Type = Entity.Type;
            }

        }
    }
}
