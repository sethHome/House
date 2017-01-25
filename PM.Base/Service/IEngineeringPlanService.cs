using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringPlan 接口
    /// </summary>
    public partial interface IEngineeringPlanService    
    {    
		
		PageSource<EngineeringPlanInfo> GetPagedList(PageQueryParam PageParam);

		EngineeringPlanEntity Get(int ID);

		int Add(EngineeringPlanInfo EngineeringPlan);

		void Update(int ID,EngineeringPlanEntity EngineeringPlan);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
