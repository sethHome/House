using Api.Framework.Core;
using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// EngineeringNote 扩展信息
    /// </summary>
    public partial class EngineeringNoteInfo
    {
        public EngineeringNoteInfo()
        {
        }

        public EngineeringNoteInfo(EngineeringNoteEntity Entity)
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.Content = Entity.Content;
            this.NoteType = Entity.NoteType;
            this.NoteDate = Entity.NoteDate;
            this.UserID = Entity.UserID;
            this.IsDeleted = Entity.IsDeleted;
        }

        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public String Content { get; set; }
        public Int32 NoteType { get; set; }
        public DateTime NoteDate { get; set; }
        public Int32 UserID { get; set; }
        public Boolean IsDeleted { get; set; }

        public List<int> AttachIDs { get; set; }

        /// <summary>
        /// 记事的接收人
        /// </summary>
        public List<int> ReceiveUsers { get; set; }

        public EngineeringEntity Engineering { get; set; }

    }
}
