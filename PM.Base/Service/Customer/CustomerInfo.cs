using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class CustomerInfo
    {
        public CustomerInfo()
        {
        }

        public CustomerInfo(CustomerEntity Entity)
        {
            this.ID = Entity.ID;
            this.Name = Entity.Name;
            this.Address = Entity.Address;
            this.Tel = Entity.Tel;
            this.IsDeleted = Entity.IsDeleted;
            this.Type = Entity.Type;
            this.LevelRate = Entity.LevelRate;
        }

        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String Tel { get; set; }
        public Boolean IsDeleted { get; set; }
        public Int32 Type { get; set; }
        public Int32 LevelRate { get; set; }
    }
}
