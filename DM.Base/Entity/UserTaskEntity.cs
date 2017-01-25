using DM.Base.Service;
using System;
using System.Collections.Generic;
namespace DM.Base.Entity
{
    /// <summary>
    /// 实体-UserTask 
    /// </summary>
    public partial class UserTaskEntity
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public Guid Identity { get; set; }
        public Guid? ProcessID { get; set; }
        public Int32 UserID { get; set; }
        public Int32 Source { get; set; }
        public Int32 Type { get; set; }
        public DateTime ReceiveDate { get; set; }
        public Int32 Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? LctDate { get; set; }
        public String Args { get; set; }
        public Boolean IsDelete { get; set; }
        public String TaskModelID { get; set; }
        public Int32 Time { get; set; }
        public String Note { get; set; }

        public UserTaskEntity()
        {
        }

        public UserTaskEntity(UserTaskInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Identity = Info.Identity;
            this.ProcessID = Info.ProcessID;
            this.UserID = Info.UserID;
            this.Source = Info.Source;
            this.Type = Info.Type;
            this.ReceiveDate = Info.ReceiveDate;
            this.Status = Info.Status;
            this.FinishDate = Info.FinishDate;
            this.LctDate = Info.LctDate;
            this.Args = Info.Args;
            this.TaskModelID = Info.TaskModelID;
            this.Time = Info.Time;
            this.Note = Info.Note;
        }

        public void SetEntity(UserTaskEntity Entity)
        {
            this.Name = Entity.Name;
            this.Identity = Entity.Identity;
            this.ProcessID = Entity.ProcessID;
            this.UserID = Entity.UserID;
            this.Source = Entity.Source;
            this.Type = Entity.Type;
            this.ReceiveDate = Entity.ReceiveDate;
            this.Status = Entity.Status;
            this.FinishDate = Entity.FinishDate;
            this.LctDate = Entity.LctDate;
            this.IsDelete = Entity.IsDelete;
            this.TaskModelID = Entity.TaskModelID;
            this.Time = Entity.Time;
            this.Note = Entity.Note;
        }
    }
}
