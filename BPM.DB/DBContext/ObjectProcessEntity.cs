using System;
using System.Collections.Generic;
namespace BPM.DB
{   
    /// <summary>
    /// 实体-ObjectProcess 
    /// </summary>
    public partial class ObjectProcessEntity    
    {    
        public Int32 ID { get; set; }
          public String ObjectKey { get; set; }
          public Int32 ObjectID { get; set; }
          public Guid ProcessID { get; set; }


		public ObjectProcessEntity()
		{
		}

		public void SetEntity(ObjectProcessEntity Entity)
		{
			if (!this.ObjectKey.Equals(Entity.ObjectKey))
			{
				this.ObjectKey = Entity.ObjectKey;
			}
			if (!this.ObjectID.Equals(Entity.ObjectID))
			{
				this.ObjectID = Entity.ObjectID;
			}
			if (!this.ProcessID.Equals(Entity.ProcessID))
			{
				this.ProcessID = Entity.ProcessID;
			}

		}
    } 
}
