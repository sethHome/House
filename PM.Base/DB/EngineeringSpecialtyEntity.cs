using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringSpecialty 
    /// </summary>
    public partial class EngineeringSpecialtyEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public Int32 Manager { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Boolean IsDone { get; set; }
        public String Note { get; set; }

        public String ProcessModel { get; set; }


        public EngineeringSpecialtyEntity()
        {
        }

        public EngineeringSpecialtyEntity(EngineeringSpecialtyInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.SpecialtyID = Info.SpecialtyID;
            this.Manager = Info.Manager;
            this.StartDate = Info.StartDate;
            this.EndDate = Info.EndDate;
            this.Note = Info.Note;
            this.ProcessModel = Info.ProcessModel;
        }

        public void SetEntity(EngineeringSpecialtyEntity Entity)
        {
            this.Manager = Entity.Manager;
            this.StartDate = Entity.StartDate;
            this.EndDate = Entity.EndDate;
            this.Note = Entity.Note;
            this.ProcessModel = Entity.ProcessModel;
        }
    }
}
