using System;
using System.Collections.Generic;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringPlan 
    /// </summary>
    public partial class EngineeringPlanEntity    
    {    
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


		public EngineeringPlanEntity()
		{
		}

		public EngineeringPlanEntity(EngineeringPlanInfo Info)
        {
			this.ID = Info.ID;
this.EngineeringID = Info.EngineeringID;
this.AccordingTo = Info.AccordingTo;
this.Range = Info.Range;
this.Product = Info.Product;
this.Note = Info.Note;
this.Input = Info.Input;
this.Principle = Info.Principle;
this.Quality = Info.Quality;
this.Measures = Info.Measures;
        }

		public void SetEntity(EngineeringPlanEntity Entity)
		{
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
    } 
}
