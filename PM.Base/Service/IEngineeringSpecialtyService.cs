using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;


namespace PM.Base
{   
    /// <summary>
    /// EngineeringSpecialty 接口
    /// </summary>
    public partial interface IEngineeringSpecialtyService    
    {    
		
		PageSource<EngineeringSpecialtyInfo> GetPagedList(PageQueryParam PageParam);

		EngineeringSpecialtyEntity Get(int ID);

        EngineeringSpecialtyInfo Get(int EngineeringID, long SpecialtyID);

        int Add(EngineeringSpecialtyInfo EngineeringSpecialty);

		void Update(int ID, List<EngineeringSpecialtyEntity> Entity);

		void Delete(int ID); 

		void Delete(string IDs);

        bool IsAllSpecialtyDone(int EngineeringID);

        void Finish(int EngineeringID, long SpecialtyID);

        List<EngineeringVolumeFileInfo> GetSpecialtyAttachs(int EngineeringID,long SpecialtyID,int UserID = 0);

        List<EngineeringVolumeEntity> GetSpecialtyVolumes(int EngineeringID, long SpecialtyID);

        List<EngineeringSpecialtyEntity> GetEngineeringSpecialtyValue(int EngineeringID);

        List<BizObject> GetEngineeringSpecialtys(int EngineeringID);
    } 
}
