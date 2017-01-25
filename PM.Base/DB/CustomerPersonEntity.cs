using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial class CustomerPersonEntity
    {
        public Int32 ID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public String Phone { get; set; }
        public String Tel { get; set; }
        public String Email { get; set; }
        public String Position { get; set; }
        public Boolean IsDeleted { get; set; }

        public void SetEntity(CustomerPersonEntity Entity)
        {
            this.Name = Entity.Name;
            this.Phone = Entity.Phone;
            this.Tel = Entity.Tel;
            this.Email = Entity.Email;
            this.Position = Entity.Position;
            this.IsDeleted = Entity.IsDeleted;
            
        }
    }
}
