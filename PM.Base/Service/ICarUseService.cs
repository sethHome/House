using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;

namespace PM.Base
{   
    /// <summary>
    /// CarUse 接口
    /// </summary>
    public partial interface ICarUseService    
    {    
		
		PageSource<CarUseInfo> GetPagedList(PageQueryParam PageParam);

		CarUseEntity Get(int ID);

        Task<int> Add(CarUseInfo CarUse);

        void Update(int ID,CarUseEntity CarUse);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
