using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class CustomerController : ApiController
    {
        [Dependency]
        public ICustomerService _ICustomerService { get; set; }

        [Route("api/v1/customer")]
        [HttpGet]
        public PageSource<CustomerInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string txtfilter = "",
            int levelrate = -1,
            bool trash = false,
            int type = 0)
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
                IsDelete = trash,
                TextCondtion = txtfilter
            };

            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("LevelRate", levelrate);

            return _ICustomerService.GetPagedList(param);
        }

        [Route("api/v1/customer/{ID}")]
        [HttpGet]
        public CustomerEntity One(int ID)
        {
            return this._ICustomerService.Get(ID);
        }

        [Route("api/v1/customer/source")]
        [HttpGet]
        public List<CustomerEntity> GetSource(string filter = "")
        {
            return this._ICustomerService.GetSource(filter);
        }

        [Route("api/v1/customer")]
        [HttpPost]
        public int Create(CustomerInfo Info)
        {
            return this._ICustomerService.Add(Info);
        }

        [Route("api/v1/customer/{ID}")]
        [HttpPut]
        public void Update(int ID, CustomerEntity Entity)
        {
            this._ICustomerService.Update(ID, Entity);
        }

        [Route("api/v1/customer/{IDs}/backup")]
        [HttpDelete]
        public void BackUp(string IDs)
        {
            this._ICustomerService.BackUp(IDs);
        }


        [Route("api/v1/customer/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._ICustomerService.Delete(ID);
            }
            else
            {
                this._ICustomerService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/customer")]
        [Route("api/v1/customer/{ID}")]
        [Route("api/v1/customer/source")]
        [Route("api/v1/customer/{IDs}/backup")]
        [HttpOptions]
        public void Options()
        {
        }

    }
}
