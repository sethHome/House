using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringSpecialtyProvide 
    /// </summary>
    public partial class EngineeringSpecialtyProvideEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public Int64 SendSpecialtyID { get; set; }
        public Int64 ReceiveSpecialtyID { get; set; }
        public Int32 SendUserID { get; set; }
        public String ReceiveUserIDs { get; set; }
        public String DocName { get; set; }
        public String DocContent { get; set; }
        public DateTime? LimitDate { get; set; }
        public Boolean IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public bool CanReceive { get; set; }


        public EngineeringSpecialtyProvideEntity()
        {
        }

        public EngineeringSpecialtyProvideEntity(EngineeringSpecialtyProvideInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.SendSpecialtyID = Info.SendSpecialtyID;
            this.ReceiveSpecialtyID = Info.ReceiveSpecialtyID;
            this.SendUserID = Info.SendUserID;
            this.ReceiveUserIDs = Info.ReceiveUserIDs;
            this.DocName = Info.DocName;
            this.DocContent = Info.DocContent;
            this.LimitDate = Info.LimitDate;
            this.IsDelete = Info.IsDelete;
            this.CreateDate = Info.CreateDate;
            this.CanReceive = Info.CanReceive;
        }

        public void SetEntity(EngineeringSpecialtyProvideEntity Entity)
        {
            this.ReceiveUserIDs = Entity.ReceiveUserIDs;
            this.DocName = Entity.DocName;
            this.DocContent = Entity.DocContent;
            this.LimitDate = Entity.LimitDate;
        }

        public void SetEntity(EngineeringSpecialtyProvideInfo Info)
        {
            this.ReceiveUserIDs = Info.ReceiveUserIDs;
            this.DocName = Info.DocName;
            this.DocContent = Info.DocContent;
            this.LimitDate = Info.LimitDate;
        }
    }
}
