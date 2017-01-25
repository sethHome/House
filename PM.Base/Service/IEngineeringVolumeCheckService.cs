using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringVolumeCheck 接口
    /// </summary>
    public partial interface IEngineeringVolumeCheckService    
    {    
		
		PageSource<EngineeringVolumeCheckEntity> GetPagedList(PageQueryParam PageParam);

        List<EngineeringVolumeCheckEntity> GetVolumeCheckList(int VolumeID);

		EngineeringVolumeCheckEntity Get(int ID);

        List<VolumeCheckGroupInfo> GetVolumeCheckStatistics(int ID);

        EngineeringVolumeCheckEntity Add(EngineeringVolumeCheckEntity Entity);

		void Update(int ID,EngineeringVolumeCheckEntity EngineeringVolumeCheck);

        void Delete(int ID); 

		void Delete(string IDs);


    } 
}
