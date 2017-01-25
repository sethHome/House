using Api.Framework.Core;
using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// EngineeringResource 扩展信息
    /// </summary>
    public partial class EngineeringResourceInfo : BizObject
    {
        public EngineeringResourceInfo():base()
        {
            base.ObjectKey = "EngineeringResource";
            base.JoinPermissionCheckUser = false;
        }

        public EngineeringResourceInfo(EngineeringResourceEntity Entity):this()
        {
            this.ID = Entity.ID;
            this.EngineeringID = Entity.EngineeringID;
            this.Name = Entity.Name;
            this.Content = Entity.Content;
            this.CreateDate = Entity.CreateDate;
            this.UserID = Entity.UserID;
            this.IsDelete = Entity.IsDelete;
        }

        private DateTime _CreateDate;
       
        public String Name { get; set; }

        public Int32 EngineeringID { get; set; }
        public String Content { get; set; }
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                _CreateDate = value;
                base.ObjectText = string.Format("收资_{0}",value.ToString("yyyy/MM/dd"));
            }
        }
        public Int32 UserID { get; set; }
        public Boolean IsDelete { get; set; }

        public List<int> AttachIDs { get; set; }

        public EngineeringEntity Engineering { get; set; }

        public override BizObject GetParent()
        {
            IEngineeringService _IEngineeringService = UnityContainerHelper.GetServer<IEngineeringService>();

            return _IEngineeringService.Get(this.EngineeringID);
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            return null;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.UserID };
        }
    }
}
