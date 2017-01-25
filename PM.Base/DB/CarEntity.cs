using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Car 
    /// </summary>
    public partial class CarEntity
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String LicensePlate { get; set; }
        public Int32 PersonCount { get; set; }
        public Int32 Status { get; set; }
        public Int32 Level { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }


        public CarEntity()
        {
        }

        public CarEntity(CarInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.LicensePlate = Info.LicensePlate;
            this.PersonCount = Info.PersonCount;
            this.Status = Info.Status;
            this.Level = Info.Level;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(CarEntity Entity)
        {
            this.Name = Entity.Name;
            this.LicensePlate = Entity.LicensePlate;
            this.PersonCount = Entity.PersonCount;
            this.Level = Entity.Level;
        }
    }
}
