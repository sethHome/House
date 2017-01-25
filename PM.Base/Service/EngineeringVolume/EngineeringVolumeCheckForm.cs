using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class EngineeringVolumeCheckForm : BizObject
    {
        public EngineeringVolumeCheckForm():base()
        {
            base.ObjectKey = "VolumeCheck";
            base.HasTask = false;
        }

        public Int32 VolumeID { get; set; }
        public Int32 Designer { get; set; }
        public Int32 Checker { get; set; }

        public List<EngineeringVolumeCheckEntity> CheckItems { get; set; }


        public override BizObject GetParent()
        {
            IEngineeringVolumeService _IEngineeringVolumeService = UnityContainerHelper.GetServer<IEngineeringVolumeService>();
            var vol = _IEngineeringVolumeService.Get(this.VolumeID);

            return new EngineeringVolumeInfo(vol);
        }

        public override List<BizObject> GetChildren(PageQueryParam PageParam)
        {
            return null;
        }

        public override int[] GetMainUsers()
        {
            return new int[] { this.Designer };
        }
       
    }
}
