using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Engineering 
    /// </summary>
    public partial class EngineeringEntity
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Number { get; set; }
        public Int32 Type { get; set; }
        public Int32 Phase { get; set; }
        public Int32 TaskType { get; set; }
        public Int32 Manager { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 ProjectID { get; set; }
        public Int32 Status { get; set; }
        public Int32 VolLevel { get; set; }


        public EngineeringEntity()
        {
        }

        public EngineeringEntity(EngineeringInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Number = Info.Number;
            this.Type = Info.Type;
            this.Phase = Info.Phase;
            this.TaskType = Info.TaskType;
            this.Manager = Info.Manager;
            this.CreateDate = Info.CreateDate;
            this.DeliveryDate = Info.DeliveryDate;
            this.FinishDate = Info.FinishDate;
            this.StartDate = Info.StartDate;
            this.StopDate = Info.StopDate;
            this.Note = Info.Note;
            this.IsDelete = Info.IsDelete;
            this.ProjectID = Info.ProjectID;
            this.Status = Info.Status;
            this.VolLevel = Info.VolLevel;
        }

        public void SetEntity(EngineeringEntity Entity)
        {
            if (!this.Name.Equals(Entity.Name))
            {
                this.Name = Entity.Name;
            }
            if (!this.Number.Equals(Entity.Number))
            {
                this.Number = Entity.Number;
            }
            if (!this.Type.Equals(Entity.Type))
            {
                this.Type = Entity.Type;
            }
            if (!this.Phase.Equals(Entity.Phase))
            {
                this.Phase = Entity.Phase;
            }
            if (!this.TaskType.Equals(Entity.TaskType))
            {
                this.TaskType = Entity.TaskType;
            }
            if (!this.Manager.Equals(Entity.Manager))
            {
                this.Manager = Entity.Manager;
            }
            if (!this.CreateDate.Equals(Entity.CreateDate))
            {
                this.CreateDate = Entity.CreateDate;
            }
            if (!this.DeliveryDate.Equals(Entity.DeliveryDate))
            {
                this.DeliveryDate = Entity.DeliveryDate;
            }
            if (!this.FinishDate.Equals(Entity.FinishDate))
            {
                this.FinishDate = Entity.FinishDate;
            }
            if (!this.StopDate.Equals(Entity.StopDate))
            {
                this.StopDate = Entity.StopDate;
            }
            if (!this.StartDate.Equals(Entity.StartDate))
            {
                this.StartDate = Entity.StartDate;
            }
            if (!this.Note.Equals(Entity.Note))
            {
                this.Note = Entity.Note;
            }
            if (!this.IsDelete.Equals(Entity.IsDelete))
            {
                this.IsDelete = Entity.IsDelete;
            }
            if (!this.ProjectID.Equals(Entity.ProjectID))
            {
                this.ProjectID = Entity.ProjectID;
            }
            if (!this.Status.Equals(Entity.Status))
            {
                this.Status = Entity.Status;
            }
            if (!this.VolLevel.Equals(Entity.VolLevel))
            {
                this.VolLevel = Entity.VolLevel;
            }

        }
    }
}
