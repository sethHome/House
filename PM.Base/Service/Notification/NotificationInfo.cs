using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// Notification 扩展信息
    /// </summary>
    public partial class NotificationInfo
    {
        public NotificationInfo()
        {
        }

        public NotificationInfo(NotificationEntity Entity)
        {
            this.ID = Entity.ID;
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
        public List<int> AttachIDs { get; set; }

        public string Head { get; set; }

    }
}
