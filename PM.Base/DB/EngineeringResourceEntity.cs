using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringResource 
    /// </summary>
    public partial class EngineeringResourceEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public String Name { get; set; }
        public String Content { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 UserID { get; set; }
        public Boolean IsDelete { get; set; }


        public EngineeringResourceEntity()
        {
        }

        public EngineeringResourceEntity(EngineeringResourceInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.Name = Info.Name;
            this.Content = Info.Content;
            this.CreateDate = Info.CreateDate;
            this.UserID = Info.UserID;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(EngineeringResourceEntity Entity)
        {
            this.Name = Entity.Name;
            this.Content = Entity.Content;
        }
    }
}
