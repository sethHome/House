using System;
using System.Collections.Generic;
namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessInstance 扩展信息
    /// </summary>
    public partial class BPMProcessInstanceInfo    
    {    
		public BPMProcessInstanceInfo()
		{
		}

		public BPMProcessInstanceInfo(BPMProcessInstanceEntity Entity)
		{
						this.ID = Entity.ID;
			this.Name = Entity.Name;
			this.Version = Entity.Version;
			this.Status = Entity.Status;
			this.CreateUser = Entity.CreateUser;
			this.StartDate = Entity.StartDate;
			this.FinishDate = Entity.FinishDate;
		}

		public Guid ID { get; set; }
          public String Name { get; set; }
          public Int32 Version { get; set; }
          public Int32 Status { get; set; }
          public Int32 CreateUser { get; set; }
          public DateTime StartDate { get; set; }
          public DateTime? FinishDate { get; set; }

		public List<int> AttachIDs { get; set; }
		
    } 
}
