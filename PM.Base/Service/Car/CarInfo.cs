using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// Car 扩展信息
    /// </summary>
    public partial class CarInfo
    {
        public CarInfo()
        {
        }

        public CarInfo(CarEntity Entity)
        {
            this.ID = Entity.ID;
            this.Name = Entity.Name;
            this.LicensePlate = Entity.LicensePlate;
            this.PersonCount = Entity.PersonCount;
            this.Status = Entity.Status;
            this.Level = Entity.Level;
            this.CreateDate = Entity.CreateDate;
            this.IsDelete = Entity.IsDelete;
        }

        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String LicensePlate { get; set; }
        public Int32 PersonCount { get; set; }
        public Int32 Status { get; set; }
        public Int32 Level { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

    }
}
