using BPM.DB;
using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// UserTask 扩展信息
    /// </summary>
    public partial class UserTaskInfo
    {
        public UserTaskInfo()
        {
        }

        public UserTaskInfo(UserTaskEntity Entity)
        {
            this.ID = Entity.ID;
            this.Name = Entity.Name;
            this.Identity = Entity.Identity;
            this.UserID = Entity.UserID;
            this.Source = Entity.Source;
            this.Type = Entity.Type;
            this.ReceiveDate = Entity.ReceiveDate;
            this.Status = Entity.Status;
            this.FinishDate = Entity.FinishDate;
            this.ProcessID = Entity.ProcessID;
            this.Args = Entity.Args;
            this.TaskModelID = Entity.TaskModelID;
            this.FinishDate = Entity.FinishDate;
            this.LctDate = Entity.LctDate;
            this.IsDelete = Entity.IsDelete;
            this.Time = Entity.Time;
            this.Note = Entity.Note;
        }

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

        /// <summary>
        /// 是否超期
        /// </summary>
        public bool IsOverdue
        {
            get
            {
                return LctDate.HasValue && LctDate.Value < FinishDate;
            }
        }

        public string ObjectKey { get; set; }

        public int ObjectID { get; set; }

        public EngineeringEntity Engineering { get; set; }

        public EngineeringVolumeEntity Volume { get; set; }

        public EngineeringSpecialtyProvideEntity SpecialtyProvide { get; set; }

    }
}
