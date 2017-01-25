using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// Calendar 扩展信息
    /// </summary>
    public partial class CalendarInfo
    {
        public CalendarInfo()
        {
        }

        public CalendarInfo(CalendarEntity Entity)
        {
            this.ID = Entity.ID;
            this.Title = Entity.Title;
            this.Type = Entity.Type;
            this.UserID = Entity.UserID;
            this.StartTime = Entity.StartTime;
            this.EndTime = Entity.EndTime;
            this.CreateUser = Entity.CreateUser;
            this.CreateDate = Entity.CreateDate;
            this.IsDelete = Entity.IsDelete;
        }

        public Int32 ID { get; set; }
        public String Title { get; set; }
        public Int32 Type { get; set; }
        public Int32 UserID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Int32 CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

    }
}
