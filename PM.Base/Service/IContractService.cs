using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Contract 接口
    /// </summary>
    public partial interface IContractService    
    {    
		
		PageSource<ContractInfo> GetPagedList(PageQueryParam PageParam);

		ContractEntity Get(int ID);

		int Add(ContractInfo Contract);

		void Update(int ID, ContractInfo Contract);

        void BackUp(string IDs);

        void Delete(int ID);

        void Delete(string IDs);
        
    } 
}
