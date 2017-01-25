using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-ContractObject 
    /// </summary>
    public partial class ContractObjectEntity
    {
        public Int32 ID { get; set; }
        public Int32 ContractID { get; set; }
        public String ObjectKey { get; set; }
        public Int32 ObjectID { get; set; }


        public ContractObjectEntity()
        {
        }


        public void SetEntity(ContractObjectEntity Entity)
        {
            if (!this.ContractID.Equals(Entity.ContractID))
            {
                this.ContractID = Entity.ContractID;
            }
            if (!this.ObjectKey.Equals(Entity.ObjectKey))
            {
                this.ObjectKey = Entity.ObjectKey;
            }
            if (!this.ObjectID.Equals(Entity.ObjectID))
            {
                this.ObjectID = Entity.ObjectID;
            }

        }
    }
}
