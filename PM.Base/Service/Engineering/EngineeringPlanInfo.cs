using System;
using System.Collections.Generic;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringPlan 扩展信息
    /// </summary>
    public partial class EngineeringPlanInfo    
    {    
		public EngineeringPlanInfo()
		{
		}

		public EngineeringPlanInfo(EngineeringPlanEntity Entity)
		{
						this.ID = Entity.ID;
			this.EngineeringID = Entity.EngineeringID;
			this.AccordingTo = Entity.AccordingTo;
			this.Range = Entity.Range;
			this.Product = Entity.Product;
			this.Note = Entity.Note;
			this.Input = Entity.Input;
			this.Principle = Entity.Principle;
			this.Quality = Entity.Quality;
			this.Measures = Entity.Measures;
		}

		public Int32 ID { get; set; }
          public Int32 EngineeringID { get; set; }
          public String AccordingTo { get; set; }
          public String Range { get; set; }
          public String Product { get; set; }
          public String Note { get; set; }
          public String Input { get; set; }
          public String Principle { get; set; }
          public String Quality { get; set; }
          public String Measures { get; set; }

		public List<int> AttachIDs { get; set; }
		
    } 
}
