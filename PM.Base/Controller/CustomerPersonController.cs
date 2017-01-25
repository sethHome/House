using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial class CustomerPersonController : ApiController
    {
        [Dependency]
        public ICustomerPersonService _ICustomerPersonService { get; set; }

        [Route("api/v1/customerperson")]
        [HttpGet]
        public PageSource<CustomerPersonEntity> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "",
            string txtfilter = "")
        {
            return _ICustomerPersonService.GetPagedList(new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
                TextCondtion = txtfilter
            });
        }

        [Route("api/v1/customer/{ID}/person")]
        [HttpPost]
        public CustomerPersonEntity One(int ID, CustomerPersonEntity Entity)
        {
            Entity.CustomerID = ID;
            this._ICustomerPersonService.Add(Entity);
            return Entity;
        }

        [Route("api/v1/customer/person/{ID}")]
        [HttpPut]
        public void Update(int ID, CustomerPersonEntity Entity)
        {
            this._ICustomerPersonService.Update(ID, Entity);
        }

        [Route("api/v1/customer/person/{ID}")]
        [HttpDelete]
        public void Delete(int ID)
        {
            this._ICustomerPersonService.Delete(ID);
        }

        [Route("api/v1/customerperson")]
        [Route("api/v1/customer/{ID}/person")]
        [Route("api/v1/customer/person/{ID}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
