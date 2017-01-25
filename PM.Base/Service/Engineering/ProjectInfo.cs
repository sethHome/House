using Api.Framework.Core;

using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public class ProjectInfo : BizObject
    {
        public ProjectInfo():base()
        {
            base.ObjectKey = "Project";
        }

        public ProjectInfo(ProjectEntity Entity):this()
        {
            this.ID = Entity.ID;
            this.Number = Entity.Number;
            this.Name = Entity.Name;
            this.Type = Entity.Type;
            this.Kind = Entity.Kind;
            this.SecretLevel = Entity.SecretLevel;
            this.VolLevel = Entity.VolLevel;
            this.Manager = Entity.Manager;
            this.CustomerID = Entity.CustomerID;
            this.CreateDate = Entity.CreateDate;
            this.DeliveryDate = Entity.DeliveryDate;
            this.Note = Entity.Note;
            this.IsDeleted = Entity.IsDeleted;
        }
       
        public String Number { get; set; }

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
        public Int32 Type { get; set; }
        public Int32 Kind { get; set; }
        public Int32 SecretLevel { get; set; }
        public Int32 VolLevel { get; set; }
        public Int32 Manager { get; set; }
        public Int32 CustomerID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public String Note { get; set; }
        public Boolean IsDeleted { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public CustomerEntity Customer { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<int> AttachIDs { get; set; }

        /// <summary>
        /// 工程
        /// </summary>
        public List<EngineeringEntity> Engineerings { get; set; }

        public override BizObject GetParent()
        {
            return null;
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            IEngineeringService _IEngineeringService = UnityContainerHelper.GetServer<IEngineeringService>();

            return _IEngineeringService.GetListByProjectID(this.ID, PageParam);
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.Manager };
        }
    }
}
