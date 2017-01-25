using Api.Framework.Core;
using BPM.DB;
using BPM.Engine;
using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// FormChange 扩展信息
    /// </summary>
    public partial class FormChangeInfo : BizObject
    {
        public FormChangeInfo():base()
        {
            base.ObjectKey = "FormChange";
            base.HasTask = true;
            base.JoinPermissionCheckUser = false;
        }

        public FormChangeInfo(FormChangeEntity Entity):this()
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.SpecialtyID = Entity.SpecialtyID;
            this.VolumeID = Entity.VolumeID;
            this.AttachID = Entity.AttachID;
            this.Reason = Entity.Reason;
            this.Content = Entity.Content;
            this.MainCustomerID = Entity.MainCustomerID;
            this.CopyCustomerID = Entity.CopyCustomerID;
            this.CreateDate = Entity.CreateDate;
            this.CreateUserID = Entity.CreateUserID;
            this.IsDelete = Entity.IsDelete;
        }

        private DateTime _CreateDate;
       
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public Int32 VolumeID { get; set; }
        public Int32 AttachID { get; set; }
        public String Reason { get; set; }
        public String Content { get; set; }
        public Int32 MainCustomerID { get; set; }
        public Int32? CopyCustomerID { get; set; }
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                _CreateDate = value;
                base.ObjectText = string.Format("变更_{0}", value.ToString("yyyy/MM/dd"));
            }
        }

        public Int32 CreateUserID { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

        public Dictionary<string, object> FlowData { get; set; }

        public EngineeringEntity Engineering { get; set; }

        public EngineeringVolumeEntity Volume { get; set; }

        public CustomerEntity MainCustomer { get; set; }

        public CustomerEntity CopyCustomer { get; set; }

        public override List<BPMTaskInstanceEntity> GetTasks()
        {
            var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();
            var _IObjectProcessService = UnityContainerHelper.GetServer<IObjectProcessService>("System3");

            var processInfo = _IObjectProcessService.Get(base.ObjectKey, this.ID);

            this.Tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == processInfo.ProcessID && t.Type == (int)TaskType.Manual && !t.IsDelete);

            return this.Tasks;
        }

        public override BizObject GetParent()
        {
            IEngineeringVolumeService _IEngineeringVolumeService = UnityContainerHelper.GetServer<IEngineeringVolumeService>();
            return new EngineeringVolumeInfo(_IEngineeringVolumeService.Get(this.VolumeID));
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            return null;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.CreateUserID };
        }
    }
}
