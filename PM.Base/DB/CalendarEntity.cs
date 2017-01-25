using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Calendar 
    /// </summary>
    public partial class CalendarEntity
    {
        public Int32 ID { get; set; }
        public String Title { get; set; }
        public Int32 Type { get; set; }
        public Int32 UserID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Int32 CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }


        public CalendarEntity()
        {
        }

        public CalendarEntity(CalendarInfo Info)
        {
            this.ID = Info.ID;
            this.Title = Info.Title;
            this.Type = Info.Type;
            this.UserID = Info.UserID;
            this.StartTime = Info.StartTime;
            this.EndTime = Info.EndTime;
            this.CreateUser = Info.CreateUser;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(CalendarEntity Entity)
        {
            this.Title = Entity.Title;
            this.Type = Entity.Type;
            this.StartTime = Entity.StartTime;
            this.EndTime = Entity.EndTime;
        }
    }
}
