using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringVolume 
    /// </summary>
    public partial class EngineeringVolumeEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public String Number { get; set; }
        public String Name { get; set; }
        public Int32 Designer { get; set; }
        public Int32 Checker { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDelete { get; set; }

        public Boolean IsDone { get; set; }
        
        public EngineeringVolumeEntity()
        {
        }

        public EngineeringVolumeEntity(EngineeringVolumeInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.SpecialtyID = Info.SpecialtyID;
            this.Number = Info.Number;
            this.Name = Info.Name;
            this.Designer = Info.Designer;
            this.Checker = Info.Checker;
            this.StartDate = Info.StartDate;
            this.EndDate = Info.EndDate;
            this.Note = Info.Note;
            this.IsDone = Info.IsDone;
        }

        public void SetEntity(EngineeringVolumeEntity Entity)
        {
            this.Number = Entity.Number;
            this.Name = Entity.Name;
            this.Designer = Entity.Designer;
            this.Checker = Entity.Checker;
            this.StartDate = Entity.StartDate;
            this.EndDate = Entity.EndDate;
            this.Note = Entity.Note;
            this.IsDelete = Entity.IsDelete;
            this.IsDone = Entity.IsDone;
        }

        public void SetEntity(EngineeringVolumeNewInfo Entity)
        {
            this.Number = Entity.Number;
            this.Name = Entity.Name;
            this.StartDate = Entity.StartDate;
            this.EndDate = Entity.EndDate;
            this.Note = Entity.Note;
        }
    }
}
