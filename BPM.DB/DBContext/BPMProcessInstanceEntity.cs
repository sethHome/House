using System;
using System.Collections.Generic;
namespace BPM.DB
{
    /// <summary>
    /// 实体-BPMProcessInstance 
    /// </summary>
    public partial class BPMProcessInstanceEntity
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public Int32 Version { get; set; }
        public Int32 Status { get; set; }
        public Int32 CreateUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Boolean IsDelete { get; set; }

        public BPMProcessInstanceEntity()
        {
        }

        public BPMProcessInstanceEntity(BPMProcessInstanceInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Version = Info.Version;
            this.Status = Info.Status;
            this.CreateUser = Info.CreateUser;
            this.StartDate = Info.StartDate;
            this.FinishDate = Info.FinishDate;
        }

        public void SetEntity(BPMProcessInstanceEntity Entity)
        {
            if (!this.Name.Equals(Entity.Name))
            {
                this.Name = Entity.Name;
            }
            if (!this.Version.Equals(Entity.Version))
            {
                this.Version = Entity.Version;
            }
            if (!this.Status.Equals(Entity.Status))
            {
                this.Status = Entity.Status;
            }
            if (!this.CreateUser.Equals(Entity.CreateUser))
            {
                this.CreateUser = Entity.CreateUser;
            }
            if (!this.StartDate.Equals(Entity.StartDate))
            {
                this.StartDate = Entity.StartDate;
            }
            if (!this.FinishDate.Equals(Entity.FinishDate))
            {
                this.FinishDate = Entity.FinishDate;
            }

        }
    }
}
