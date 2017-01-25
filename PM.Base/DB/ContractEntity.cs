using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Contract 
    /// </summary>
    public partial class ContractEntity
    {
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
       

        public ContractEntity()
        {
        }

        public ContractEntity(ContractInfo Info)
        {
            this.ID = Info.ID;
            this.Number = Info.Number;
            this.Name = Info.Name;
            this.CustomerID = Info.CustomerID;
            this.SignDate = Info.SignDate;
            this.Fee = Info.Fee;
            this.Type = Info.Type;
            this.Status = Info.Status;
            this.Note = Info.Note;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(ContractEntity Entity)
        {
            if (!this.Number.Equals(Entity.Number))
            {
                this.Number = Entity.Number;
            }
            if (!this.Name.Equals(Entity.Name))
            {
                this.Name = Entity.Name;
            }
            if (!this.CustomerID.Equals(Entity.CustomerID))
            {
                this.CustomerID = Entity.CustomerID;
            }
            if (!this.SignDate.Equals(Entity.SignDate))
            {
                this.SignDate = Entity.SignDate;
            }
            if (!this.Fee.Equals(Entity.Fee))
            {
                this.Fee = Entity.Fee;
            }
            if (!this.Type.Equals(Entity.Type))
            {
                this.Type = Entity.Type;
            }
            if (!this.Status.Equals(Entity.Status))
            {
                this.Status = Entity.Status;
            }
            if (!this.Note.Equals(Entity.Note))
            {
                this.Note = Entity.Note;
            }
            if (!this.CreateDate.Equals(Entity.CreateDate))
            {
                this.CreateDate = Entity.CreateDate;
            }
            if (!this.IsDelete.Equals(Entity.IsDelete))
            {
                this.IsDelete = Entity.IsDelete;
            }

        }
    }
}
