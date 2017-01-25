using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-FormChange 
    /// </summary>
    public partial class FormChangeEntity
    {
        public Int32 ID { get; set; }
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public Int32 VolumeID { get; set; }
        public Int32 AttachID { get; set; }
        public String Reason { get; set; }
        public String Content { get; set; }
        public Int32 MainCustomerID { get; set; }
        public Int32? CopyCustomerID { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 CreateUserID { get; set; }
        public Boolean IsDelete { get; set; }


        public FormChangeEntity()
        {
        }

        public FormChangeEntity(FormChangeInfo Info)
        {
            this.ID = Info.ID;
            this.EngineeringID = Info.EngineeringID;
            this.SpecialtyID = Info.SpecialtyID;
            this.VolumeID = Info.VolumeID;
            this.AttachID = Info.AttachID;
            this.Reason = Info.Reason;
            this.Content = Info.Content;
            this.MainCustomerID = Info.MainCustomerID;
            this.CopyCustomerID = Info.CopyCustomerID;
            this.CreateDate = Info.CreateDate;
            this.CreateUserID = Info.CreateUserID;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(FormChangeEntity Entity)
        {
            this.EngineeringID = Entity.EngineeringID;
            this.SpecialtyID = Entity.SpecialtyID;
            this.VolumeID = Entity.VolumeID;
            this.AttachID = Entity.AttachID;
            this.Reason = Entity.Reason;
            this.Content = Entity.Content;
            this.MainCustomerID = Entity.MainCustomerID;
            this.CopyCustomerID = Entity.CopyCustomerID;
            //this.CreateDate = Entity.CreateDate;
            //this.CreateUserID = Entity.CreateUserID;
            //this.IsDelete = Entity.IsDelete;

        }
    }
}
