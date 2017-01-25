using System;
using System.Collections.Generic;
using Api.Framework.Core;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolumeCheck 扩展信息
    /// </summary>
    public partial class EngineeringVolumeCheckInfo 
    {
        public EngineeringVolumeCheckInfo()
        {
          
        }

        public EngineeringVolumeCheckInfo(EngineeringVolumeCheckEntity Entity) : this()
        {
            this.ID = Entity.ID;
            this.VolumeID = Entity.VolumeID;
            this.Context = Entity.Context;
            this.Type = Entity.Type;
            this.UserID = Entity.UserID;
            this.Date = Entity.Date;
            this.IsCorrect = Entity.IsCorrect;
            this.IsPass = Entity.IsPass;
            this.IsDelete = Entity.IsDelete;
        }
        public Int32 ID { get; set; }
        public Int32 VolumeID { get; set; }
        public String Context { get; set; }
        public Int32 Type { get; set; }
        public Int32 UserID { get; set; }
        public DateTime Date { get; set; }
        public Boolean IsCorrect { get; set; }
        public Boolean IsPass { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

    }
}
