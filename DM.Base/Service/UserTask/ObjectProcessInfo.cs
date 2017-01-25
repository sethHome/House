using BPM.DB;
using DM.Base.Entity;
using System;
using System.Collections.Generic;
namespace DM.Base.Service
{   
    /// <summary>
    /// ObjectProcess 扩展信息
    /// </summary>
    public partial class ObjectProcessInfo    
    {    
		public ObjectProcessInfo()
		{
		}

		public ObjectProcessInfo(ObjectProcessEntity Entity)
		{
						this.ID = Entity.ID;
			this.ObjectKey = Entity.ObjectKey;
			this.ObjectID = Entity.ObjectID;
			this.ProcessID = Entity.ProcessID;
		}

		public Int32 ID { get; set; }
          public String ObjectKey { get; set; }
          public Int32 ObjectID { get; set; }
          public Guid ProcessID { get; set; }

		public List<int> AttachIDs { get; set; }
		
    } 
}
