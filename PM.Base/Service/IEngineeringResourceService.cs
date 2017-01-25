using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringResource 接口
    /// </summary>
    public partial interface IEngineeringResourceService    
    {    
		
		PageSource<EngineeringResourceInfo> GetPagedList(PageQueryParam PageParam);

		EngineeringResourceEntity Get(int ID);

		int Add(EngineeringResourceInfo EngineeringResource);

		void Update(int ID,EngineeringResourceEntity EngineeringResource);

		void Delete(int ID); 

		void Delete(string IDs);

        List<BizObject> GetEngineeringResource(int EngineeringID);
    } 
}
