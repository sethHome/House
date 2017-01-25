using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// EngineeringSpecialtyProvide 扩展信息
    /// </summary>
    public partial class EngineeringSpecialtyProvideInfo
    {
        public EngineeringSpecialtyProvideInfo()
        {
        }

        public EngineeringSpecialtyProvideInfo(EngineeringSpecialtyProvideEntity Entity)
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.SendSpecialtyID = Entity.SendSpecialtyID;
            this.ReceiveSpecialtyID = Entity.ReceiveSpecialtyID;
            this.SendUserID = Entity.SendUserID;
            this.ReceiveUserIDs = Entity.ReceiveUserIDs;
            this.DocName = Entity.DocName;
            this.DocContent = Entity.DocContent;
            this.LimitDate = Entity.LimitDate;
            this.IsDelete = Entity.IsDelete;
            this.CreateDate = Entity.CreateDate;
            this.CanReceive = Entity.CanReceive;
        }

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

        public Int32 ApproveUser { get; set; }

        public List<int> AttachIDs { get; set; }

        public List<int> VolumeFiles { get; set; }

        public EngineeringEntity Engineering { get; set; }

        public EngineeringSpecialtyEntity Specialty { get; set; }

    }
}
