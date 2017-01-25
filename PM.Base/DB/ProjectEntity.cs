using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public partial class ProjectEntity
    {
        public Int32 ID { get; set; }
        public String Number { get; set; }
        public String Name { get; set; }
        public Int32 Type { get; set; }
        public Int32 Kind { get; set; }
        public Int32 SecretLevel { get; set; }
        public Int32 VolLevel { get; set; }
        public Int32 Manager { get; set; }
        public Int32 CustomerID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDeleted { get; set; }


        public ProjectEntity() { }

        public ProjectEntity(ProjectInfo Info)
        {
            this.Number = Info.Number;
            this.Name = Info.Name;
            this.Type = Info.Type;
            this.SecretLevel = Info.SecretLevel;
            this.VolLevel = Info.VolLevel;
            this.Manager = Info.Manager;
            this.CustomerID = Info.CustomerID;
            this.CreateDate = Info.CreateDate;
            this.DeliveryDate = Info.DeliveryDate;
            this.Note = Info.Note;
            this.IsDeleted = Info.IsDeleted;
            this.Kind = Info.Kind;
        }

        public void SetEntity(ProjectEntity Entity)
        {
            if (!this.Number.Equals(Entity.Number))
            {
                this.Number = Entity.Number;
            }
            if (!this.Name.Equals(Entity.Name))
            {
                this.Name = Entity.Name;
            }
            if (!this.Type.Equals(Entity.Type))
            {
                this.Type = Entity.Type;
            }
            if (!this.Kind.Equals(Entity.Kind))
            {
                this.Kind = Entity.Kind;
            }
            if (!this.SecretLevel.Equals(Entity.SecretLevel))
            {
                this.SecretLevel = Entity.SecretLevel;
            }
            if (!this.VolLevel.Equals(Entity.VolLevel))
            {
                this.VolLevel = Entity.VolLevel;
            }
            if (!this.Manager.Equals(Entity.Manager))
            {
                this.Manager = Entity.Manager;
            }
            if (!this.CustomerID.Equals(Entity.CustomerID))
            {
                this.CustomerID = Entity.CustomerID;
            }
            if (!this.CreateDate.Equals(Entity.CreateDate))
            {
                this.CreateDate = Entity.CreateDate;
            }
            if (!this.DeliveryDate.Equals(Entity.DeliveryDate))
            {
                this.DeliveryDate = Entity.DeliveryDate;
            }
            if (!this.Note.Equals(Entity.Note))
            {
                this.Note = Entity.Note;
            }
            if (!this.IsDeleted.Equals(Entity.IsDeleted))
            {
                this.IsDeleted = Entity.IsDeleted;
            }

        }
    }
}
