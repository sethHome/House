using System;
using System.Collections.Generic;
namespace BPM.DB
{
    /// <summary>
    /// 实体-BPMTaskInstance 
    /// </summary>
    public partial class BPMTaskInstanceEntity
    {
        public Guid ID { get; set; }

        public Guid ProcessID { get; set; }
        public String Name { get; set; }
        public Guid? Source { get; set; }
        public Guid? Target { get; set; }
        public Int32 Status { get; set; }
        public Int32 UserID { get; set; }
        public String Users { get; set; }
        public Int32 Type { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public DateTime? TurnDate { get; set; }
        public Boolean IsDelete { get; set; }
        public String SourceID { get; set; }
        public DateTime? EstDate { get; set; }
        public DateTime? LctDate { get; set; }

        public BPMTaskInstanceEntity()
        {
        }

        public BPMTaskInstanceEntity(BPMTaskInstanceInfo Info)
        {
            this.ID = Info.ID;
            this.ProcessID = Info.ProcessID;
            this.Name = Info.Name;
            this.Source = Info.Source;
            this.Target = Info.Target;
            this.Status = Info.Status;
            this.UserID = Info.UserID;
            this.Users = Info.Users;
            this.Type = Info.Type;
            this.ExecuteDate = Info.ExecuteDate;
            this.TurnDate = Info.TurnDate;
            this.SourceID = Info.SourceID;
            this.EstDate = Info.EstDate;
            this.LctDate = Info.LctDate;
        }

        public void SetEntity(BPMTaskInstanceEntity Entity)
        {
            this.Status = Entity.Status;
            this.UserID = Entity.UserID;
            this.Users = Entity.Users;
            this.ExecuteDate = Entity.ExecuteDate;
            this.TurnDate = Entity.TurnDate;
            this.EstDate = Entity.EstDate;
            this.LctDate = Entity.LctDate;
        }
    }
}
