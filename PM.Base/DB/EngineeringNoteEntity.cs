using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringNote 
    /// </summary>
    public partial class EngineeringNoteEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public String Content { get; set; }
        public Int32 NoteType { get; set; }
        public DateTime NoteDate { get; set; }
        public Int32 UserID { get; set; }
        public Boolean IsDeleted { get; set; }

        public EngineeringNoteEntity()
        {
        }

        public EngineeringNoteEntity(EngineeringNoteInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.Content = Info.Content;
            this.NoteType = Info.NoteType;
            this.NoteDate = Info.NoteDate;
            this.UserID = Info.UserID;
            this.IsDeleted = Info.IsDeleted;
            
        }

        public void SetEntity(EngineeringNoteEntity Entity)
        {
            this.Content = Entity.Content;
            this.NoteType = Entity.NoteType;
        }
    }
}
