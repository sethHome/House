using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Notification 
    /// </summary>
    public partial class NotificationEntity
    {
        public Int32 ID { get; set; }
        public String Title { get; set; }
        public String Info { get; set; }
        public Int32 ReceiveUser { get; set; }
        public Int32 SendUser { get; set; }
        public DateTime EffectDate { get; set; }
        public DateTime? InvalidDate { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32? SourceID { get; set; }
        public String SourceName { get; set; }
        public String SourceTag { get; set; }
        public Boolean IsRead { get; set; }

        public NotificationEntity()
        {
        }

        public NotificationEntity(NotificationInfo Info)
        {
            this.ID = Info.ID;
            this.Title = Info.Title;
            this.Info = Info.Info;
            this.ReceiveUser = Info.ReceiveUser;
            this.SendUser = Info.SendUser;
            this.EffectDate = Info.EffectDate;
            this.InvalidDate = Info.InvalidDate;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
            this.SourceID = Info.SourceID;
            this.SourceName = Info.SourceName;
            this.SourceTag = Info.SourceTag;
            this.IsRead = Info.IsRead;
        }

        public void SetEntity(NotificationEntity Entity)
        {
            this.Title = Entity.Title;
            this.Info = Entity.Info;
            this.ReceiveUser = Entity.ReceiveUser;
            this.SendUser = Entity.SendUser;
            this.EffectDate = Entity.EffectDate;
            this.InvalidDate = Entity.InvalidDate;
            this.CreateDate = Entity.CreateDate;
            this.IsDelete = Entity.IsDelete;
            this.SourceID = Entity.SourceID;
            this.SourceName = Entity.SourceName;
            this.SourceTag = Entity.SourceTag;
            this.IsRead = Entity.IsRead;
        }
    }
}
