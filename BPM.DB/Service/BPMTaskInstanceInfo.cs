using System;
using System.Collections.Generic;
namespace BPM.DB
{
    /// <summary>
    /// BPMTaskInstance 扩展信息
    /// </summary>
    public partial class BPMTaskInstanceInfo
    {
        public BPMTaskInstanceInfo()
        {
        }

        public BPMTaskInstanceInfo(BPMTaskInstanceEntity Entity)
        {
            this.ID = Entity.ID;
            this.ProcessID = Entity.ProcessID;
            this.Name = Entity.Name;
            this.Source = Entity.Source;
            this.Target = Entity.Target;
            this.Status = Entity.Status;
            this.UserID = Entity.UserID;
            this.Users = Entity.Users;
            this.Type = Entity.Type;
            this.ExecuteDate = Entity.ExecuteDate;
            this.TurnDate = Entity.TurnDate;
            this.SourceID = Entity.SourceID;
            this.EstDate = Entity.EstDate;
            this.LctDate = Entity.LctDate;
        }

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
        public String SourceID { get; set; }
        public DateTime? EstDate { get; set; }
        public DateTime? LctDate { get; set; }
        public List<int> AttachIDs { get; set; }
    }
}
