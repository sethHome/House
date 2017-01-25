using System;
using System.Collections.Generic;
namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessTaskIDMap 扩展信息
    /// </summary>
    public partial class BPMProcessTaskIDMapInfo    
    {    
		public BPMProcessTaskIDMapInfo()
		{
		}

		public BPMProcessTaskIDMapInfo(BPMProcessTaskIDMapEntity Entity)
		{
						this.ID = Entity.ID;
			this.ProcessID = Entity.ProcessID;
			this.TaskKey = Entity.TaskKey;
			this.TaskID = Entity.TaskID;
		}

		public Int32 ID { get; set; }
          public Guid ProcessID { get; set; }
          public String TaskKey { get; set; }
          public Guid TaskID { get; set; }

		public List<int> AttachIDs { get; set; }
		
    } 
}
