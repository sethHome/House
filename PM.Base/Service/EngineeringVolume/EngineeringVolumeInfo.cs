using Api.Framework.Core;
using Api.Framework.Core.Organization;
using System;
using System.Collections.Generic;
using BPM.DB;
using BPM.Engine;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolume 扩展信息
    /// </summary>
    public partial class EngineeringVolumeInfo : BizObject
    {
        public EngineeringVolumeInfo() : base()
        {
            base.ObjectKey = "Volume";
            base.HasTask = true;
        }

        public EngineeringVolumeInfo(EngineeringVolumeEntity Entity) : this()
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.SpecialtyID = Entity.SpecialtyID;
            this.Number = Entity.Number;
            this.Name = Entity.Name;
            this.Designer = Entity.Designer;
            this.Checker = Entity.Checker;
            this.StartDate = Entity.StartDate;
            this.EndDate = Entity.EndDate;
            this.FinishDate = Entity.FinishDate;
            this.Note = Entity.Note;
            this.IsDone = Entity.IsDone;
        }

        public EngineeringVolumeInfo(EngineeringProductionInfo Info) : this()
        {
            if (Info.Volume != null)
            {
                this.ID = Info.Volume.ID;
                this.EngineeringID = Info.Volume.EngineeringID;
                this.SpecialtyID = Info.Volume.SpecialtyID;
                this.Number = Info.Volume.Number;
                this.Name = Info.Volume.Name;
                this.Designer = Info.Volume.Designer;
                this.Checker = Info.Volume.Checker;
                this.StartDate = Info.Volume.StartDate;
                this.EndDate = Info.Volume.EndDate;
                this.FinishDate = Info.Volume.FinishDate;
                this.Note = Info.Volume.Note;
                this.IsDone = Info.Volume.IsDone;
            }

            this.Engineering = Info.Engineering;
            this.Project = Info.Project;
            this.Specialty = Info.Specialty;
        }

        private String _Name;
        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }
        public String Number { get; set; }
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                base.ObjectText = value;
            }
        }
        public Int32 Designer { get; set; }
        public Int32 Checker { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDone { get; set; }

        /// <summary>
        /// 是否超期
        /// </summary>
        public bool IsTimeOut
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return (FinishDate.HasValue ? FinishDate.Value : DateTime.Now) > EndDate.Value;
                }

                return false;
            }
        }

        public List<int> AttachIDs { get; set; }

        public EngineeringEntity Engineering { get; set; }

        public ProjectEntity Project { get; set; }

        public EngineeringSpecialtyEntity Specialty { get; set; }

        public List<EngineeringVolumeEntity> Volumes { get; set; }

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
            IEngineeringSpecialtyService _IEngineeringSpecialtyService = UnityContainerHelper.GetServer<IEngineeringSpecialtyService>();
            return _IEngineeringSpecialtyService.Get(this.EngineeringID, this.SpecialtyID);
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            IFormChangeService _IFormChangeService = UnityContainerHelper.GetServer<IFormChangeService>();
            var result = _IFormChangeService.GetVolumeChanges(this.ID);

            IEngineeringVolumeCheckService _IEngineeringVolumeCheckService = UnityContainerHelper.GetServer<IEngineeringVolumeCheckService>();
            var checkList = _IEngineeringVolumeCheckService.GetVolumeCheckList(this.ID);
            if (checkList.Count > 0)
            {
                result.Add(new EngineeringVolumeCheckForm()
                {
                    ID = this.ID,
                    ObjectText = "校审单",
                    VolumeID = this.ID,
                    Checker = this.Checker,
                    Designer = this.Designer,
                    CheckItems = checkList
                });
            }

            return result;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.Designer };
        }
    }
}
