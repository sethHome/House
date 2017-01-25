using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-CarUse 
    /// </summary>
    public partial class CarUseEntity
    {
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


        public CarUseEntity()
        {
        }

        public CarUseEntity(CarUseInfo Info)
        {
            this.ID = Info.ID;
            this.CarID = Info.CarID;
            this.StartDate = Info.StartDate;
            this.BackDate = Info.BackDate;
            this.Manager = Info.Manager;
            this.TargetPlace = Info.TargetPlace;
            this.Mileage = Info.Mileage;
            this.PeerStaff = Info.PeerStaff;
            this.PeerStaffCount = Info.PeerStaffCount;
            this.Explain = Info.Explain;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(CarUseEntity Entity)
        {
            this.CarID = Entity.CarID;
            this.StartDate = Entity.StartDate;
            this.BackDate = Entity.BackDate;
            this.Manager = Entity.Manager;
            this.TargetPlace = Entity.TargetPlace;
            this.Mileage = Entity.Mileage;
            this.PeerStaff = Entity.PeerStaff;
            this.PeerStaffCount = Entity.PeerStaff.Split(',').Length;
            this.Explain = Entity.Explain;

        }
    }
}
