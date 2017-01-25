using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class CustomerEntity
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String Tel { get; set; }
        public Boolean IsDeleted { get; set; }
        public Int32 Type { get; set; }
        public Int32 LevelRate { get; set; }
        

        public CustomerEntity()
        { }

        public CustomerEntity(CustomerInfo Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Address = Info.Address;
            this.Tel = Info.Tel;
            this.IsDeleted = Info.IsDeleted;
            this.Type = Info.Type;
            this.LevelRate = Info.LevelRate;
        }

        public void SetEntity(CustomerEntity Entity)
        {
            this.Name = Entity.Name;
            this.Address = Entity.Address;
            this.Tel = Entity.Tel;
            this.LevelRate = Entity.LevelRate;
        }
    }
}
