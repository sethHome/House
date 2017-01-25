using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-EngineeringVolumeCheck 
    /// </summary>
    public partial class EngineeringVolumeCheckEntity
    {
        public Int32 ID { get; set; }
        public Int32 VolumeID { get; set; }
        public String Context { get; set; }
        public Int32 Type { get; set; }
        public Int32 UserID { get; set; }
        public DateTime Date { get; set; }
        public Boolean IsCorrect { get; set; }
        public Boolean IsPass { get; set; }
        public Boolean IsDelete { get; set; }


        public EngineeringVolumeCheckEntity()
        {
        }

        public EngineeringVolumeCheckEntity(EngineeringVolumeCheckInfo Info)
        {
            this.ID = Info.ID;
            this.VolumeID = Info.VolumeID;
            this.Context = Info.Context;
            this.Type = Info.Type;
            this.UserID = Info.UserID;
            this.Date = Info.Date;
            this.IsCorrect = Info.IsCorrect;
            this.IsPass = Info.IsPass;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(EngineeringVolumeCheckEntity Entity)
        {
            if (!this.Context.Equals(Entity.Context))
            {
                this.Context = Entity.Context;
            }
            if (!this.Type.Equals(Entity.Type))
            {
                this.Type = Entity.Type;
            }
            if (!this.UserID.Equals(Entity.UserID))
            {
                this.UserID = Entity.UserID;
            }
            if (!this.Date.Equals(Entity.Date))
            {
                this.Date = Entity.Date;
            }
            if (!this.IsCorrect.Equals(Entity.IsCorrect))
            {
                this.IsCorrect = Entity.IsCorrect;
            }
            if (!this.IsPass.Equals(Entity.IsPass))
            {
                this.IsPass = Entity.IsPass;
            }
        }
    }
}
