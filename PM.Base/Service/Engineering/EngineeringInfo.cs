using Api.Framework.Core;

using System;
using System.Collections.Generic;

namespace PM.Base
{
    /// <summary>
    /// Engineering 扩展信息
    /// </summary>
    public partial class EngineeringInfo : BizObject
    {
        public EngineeringInfo():base()
        {
            base.ObjectKey = "Engineering";
        }

        public EngineeringInfo(EngineeringEntity Entity):this()
        {
            this.ID = Entity.ID;
            this.Name = Entity.Name;
            this.Number = Entity.Number;
            this.Type = Entity.Type;
            this.Phase = Entity.Phase;
            this.TaskType = Entity.TaskType;
            this.Manager = Entity.Manager;
            this.CreateDate = Entity.CreateDate;
            this.DeliveryDate = Entity.DeliveryDate;
            this.FinishDate = Entity.FinishDate;
            this.StartDate = Entity.StartDate;
            this.StopDate = Entity.StopDate;
            this.Note = Entity.Note;
            this.IsDelete = Entity.IsDelete;
            this.ProjectID = Entity.ProjectID;
            this.Status = Entity.Status;
            this.VolLevel = Entity.VolLevel;
        }

        private String _Name;
       
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
        public String Number { get; set; }
        public Int32 Type { get; set; }
        public Int32 Phase { get; set; }
        public Int32 TaskType { get; set; }
        public Int32 Manager { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 CreateUser { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 ProjectID { get; set; }
        public Int32 Status { get; set; }
        public Int32 VolLevel { get; set; }
        /// <summary>
        /// 工程历时天数
        /// </summary>
        public int DuringDay
        {
            get
            {
                switch (this.Status)
                {
                    case (int)EnumEngineeringStatus.完成: return (int)(this.FinishDate - this.StartDate).Value.TotalDays;
                    case (int)EnumEngineeringStatus.暂停: return (int)(this.StopDate - this.StartDate).Value.TotalDays;
                    case (int)EnumEngineeringStatus.启动: return (int)(DateTime.Now - this.StartDate).Value.TotalDays;
                   
                    default:
                        return 0;
                }
            }
        }
        /// <summary>
        /// 是否超期
        /// </summary>
        public bool IsTimeOut
        {
            get
            {
                if (this.DeliveryDate != null && this.Status != (int)EnumEngineeringStatus.暂停)
                {
                    // 设置了工程的计划完成时间，并且工程没有被暂停

                    return (this.FinishDate.HasValue ? this.FinishDate : DateTime.Now) > this.DeliveryDate;
                }

                return false;
            }
        }

        public List<int> AttachIDs { get; set; }

        public ProjectEntity ProjectInfo { get; set; }

        public override BizObject GetParent()
        {
            IProjectService _IProjectService = UnityContainerHelper.GetServer<IProjectService>();

            return new ProjectInfo(_IProjectService.Get(this.ProjectID));
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            IEngineeringSpecialtyService _IEngineeringSpecialtyService = UnityContainerHelper.GetServer<IEngineeringSpecialtyService>();
            IEngineeringResourceService _IEngineeringResourceService = UnityContainerHelper.GetServer<IEngineeringResourceService>();

            var specils = _IEngineeringSpecialtyService.GetEngineeringSpecialtys(this.ID);
            var resource = _IEngineeringResourceService.GetEngineeringResource(this.ID);
            specils.AddRange(resource);

            return specils;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.Manager };
        }
    }
}
