using Api.Framework.Core;

using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// EngineeringSpecialty 扩展信息
    /// </summary>
    public partial class EngineeringSpecialtyInfo : BizObject
    {
        public EngineeringSpecialtyInfo():base()
        {
            base.ObjectKey = "Specialty";
        }

        public EngineeringSpecialtyInfo(EngineeringSpecialtyEntity Entity):this()
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.SpecialtyID = Entity.SpecialtyID;
            this.Manager = Entity.Manager;
            this.StartDate = Entity.StartDate;
            this.EndDate = Entity.EndDate;
            this.FinishDate = Entity.FinishDate;
            this.Note = Entity.Note;
            this.ProcessModel = Entity.ProcessModel;
            this.IsDone = Entity.IsDone;
        }

        public EngineeringSpecialtyInfo(EngineeringProductionInfo info) : this()
        {
            if (info.Specialty != null)
            {
                this.ID = info.Specialty.ID;
                this.EngineeringID = info.Specialty.EngineeringID;
                this.SpecialtyID = info.Specialty.SpecialtyID;
                this.Manager = info.Specialty.Manager;
                this.StartDate = info.Specialty.StartDate;
                this.EndDate = info.Specialty.EndDate;
                this.FinishDate = info.Specialty.FinishDate;
                this.Note = info.Specialty.Note;
                this.ProcessModel = info.Specialty.ProcessModel;
                this.IsDone = info.Specialty.IsDone;
            }
        }

        public Int32 EngineeringID { get; set; }
        public Int64 SpecialtyID { get; set; }

        public Int32 Manager { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public String Note { get; set; }
        public String ProcessModel { get; set; }

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

        public List<EngineeringSpecialtyEntity> Specialtys { get; set; }

        public override BizObject GetParent()
        {
            IEngineeringService _IEngineeringService = UnityContainerHelper.GetServer<IEngineeringService>();

            return _IEngineeringService.Get(this.EngineeringID);
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            IEngineeringVolumeService _IEngineeringVolumeService = UnityContainerHelper.GetServer<IEngineeringVolumeService>();

            var volumes = _IEngineeringVolumeService.GetSpecialtyVolumesV2(this.EngineeringID, this.SpecialtyID, PageParam != null && PageParam.FilterCondtion.ContainsKey("Task"));

            var result = new List<BizObject>();

            foreach (var item in volumes)
            {
                result.Add(item);
            }

            return result;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.Manager };
        }
    }
}
