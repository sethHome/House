using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;

namespace PM.Base
{   
    /// <summary>
    /// EngineeringSpecialtyProvide 接口
    /// </summary>
    public partial interface IEngineeringSpecialtyProvideService    
    {    
		
		PageSource<EngineeringSpecialtyProvideInfo> GetPagedList(PageQueryParam PageParam);

		EngineeringSpecialtyProvideEntity Get(int ID);

		Task<int> Add(EngineeringSpecialtyProvideInfo EngineeringSpecialtyProvide);

        void CanReceive(int ID);

		void Update(int ID, EngineeringSpecialtyProvideInfo Info);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
