using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial class CustomerPersonInfo
    {
        public CustomerPersonInfo()
        {
        }

        public CustomerPersonInfo(CustomerPersonEntity Entity)
        {
            this.ID = Entity.ID;
            this.CustomerID = Entity.CustomerID;
            this.Name = Entity.Name;
            this.Phone = Entity.Phone;
            this.Tel = Entity.Tel;
            this.Email = Entity.Email;
            this.Position = Entity.Position;
            this.IsDeleted = Entity.IsDeleted;
        }

        public Int32 ID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public String Phone { get; set; }
        public String Tel { get; set; }
        public String Email { get; set; }
        public String Position { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
