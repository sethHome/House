using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// CarUse 扩展信息
    /// </summary>
    public partial class CarUseInfo
    {
        public CarUseInfo()
        {
        }

        public CarUseInfo(CarUseEntity Entity)
        {
            this.ID = Entity.ID;
            this.CarID = Entity.CarID;
            this.StartDate = Entity.StartDate;
            this.BackDate = Entity.BackDate;
            this.Manager = Entity.Manager;
            this.TargetPlace = Entity.TargetPlace;
            this.Mileage = Entity.Mileage;
            this.PeerStaff = Entity.PeerStaff;
            this.PeerStaffCount = Entity.PeerStaffCount;
            this.Explain = Entity.Explain;
            this.CreateDate = Entity.CreateDate;
            this.IsDelete = Entity.IsDelete;
        }

        public Int32 ID { get; set; }
        public Int32 CarID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime BackDate { get; set; }
        public Int32 Manager { get; set; }
        public String TargetPlace { get; set; }
        public Decimal Mileage { get; set; }
        public String PeerStaff { get; set; }
        public Int32? PeerStaffCount { get; set; }
        public String Explain { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }

        public Dictionary<string, object> FlowData { get; set; }

        public List<int> AttachIDs { get; set; }

    }
}
