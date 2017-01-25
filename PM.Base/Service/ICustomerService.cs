using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial interface ICustomerService
    {
        PageSource<CustomerInfo> GetPagedList(PageQueryParam PageParam);

        CustomerEntity Get(int ID);

        List<CustomerEntity> GetSource(string filter);

        int Add(CustomerInfo Customer);

        void Update(int ID, CustomerEntity Customer);

        void BackUp(string IDs);

        void Delete(int ID);

        void Delete(string IDs);
    }
}
