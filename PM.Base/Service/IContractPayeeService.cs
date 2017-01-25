using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// ContractPayee 接口
    /// </summary>
    public partial interface IContractPayeeService    
    {

        List<ContractPayeeInfo> GetList(int ContractID);

		ContractPayeeEntity Get(int ID);

		int Add(int ID , ContractPayeeInfo ContractPayee);

		void Update(int ID,ContractPayeeEntity ContractPayee);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
