using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial interface ICustomerPersonService    
    {    
		
		PageSource<CustomerPersonEntity> GetPagedList(PageQueryParam PageParam);

		CustomerPersonEntity Get(int ID);

		void Add(CustomerPersonEntity CustomerPerson);

		void Update(int ID,CustomerPersonEntity CustomerPerson);

		void Delete(int ID); 
    } 
}
