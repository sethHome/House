
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Contract Controlle
    /// </summary>
    public partial class ContractController : ApiController
    {   
		[Dependency]
		public IContractService _IContractService { get; set; }

        [Dependency]
        public IContractPayeeService _IContractPayeeService { get; set; }

        [Route("api/v1/contract")]
        [HttpGet]
        public PageSource<ContractInfo> All(
			int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
			string orderdirection = "desc",
			string txtfilter = "",
            bool trash = false,
            int type = 0,int status = 0,int customer = 0,
            string signdateto = "",string signdatefrom = "",
            string createdateto = "",string createdatefrom = "")
        {
			var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
				IsDesc = orderdirection.ToLower().Equals("desc"),
                TextCondtion = txtfilter,
                IsDelete = trash,
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Customer", customer);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("SigndateTo", signdateto);
            param.FilterCondtion.Add("SigndateFrom", signdatefrom);

            return _IContractService.GetPagedList(param);
        }

		[Route("api/v1/contract/{ID}")]
        [HttpGet]
        public ContractEntity One(int ID)
        {
            return this._IContractService.Get(ID);
        }

		[Route("api/v1/contract")]
        [HttpPost]
        public int Create(ContractInfo Info)
        {
           return this._IContractService.Add(Info);
        }

		[Route("api/v1/contract/{ID}")]
        [HttpPut]
        public void Update(int ID, ContractInfo Entity)
        {
            this._IContractService.Update(ID, Entity);
        }

        [Route("api/v1/contract/{IDs}/backup")]
        [HttpDelete]
        public void BackUp(string IDs)
        {
            this._IContractService.BackUp(IDs);
        }

        [Route("api/v1/contract/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
			if (ID.Contains(","))
            {
                this._IContractService.Delete(ID);
            }
            else
            {
                this._IContractService.Delete(int.Parse(ID));
            }
        }

        /// <summary>
        /// 合同收费
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        [Route("api/v1/contract/{ID}/payee")]
        [HttpGet]
        public List<ContractPayeeInfo> GetContractPayees(int ID)
        {
            return _IContractPayeeService.GetList(ID);
        }


        /// <summary>
        /// 合同收费
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        [Route("api/v1/contract/{ID}/payee")]
        [HttpPost]
        public int AddPayee(int ID,ContractPayeeInfo Info)
        {
            return _IContractPayeeService.Add(ID, Info);
        }

        /// <summary>
        /// 合同收费
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        [Route("api/v1/contract/payee/{ID}")]
        [HttpDelete]
        public void DeletePayee(int ID)
        {
             _IContractPayeeService.Delete(ID);
        }

        [Route("api/v1/contract")]
        [Route("api/v1/contract/{ID}")]
        [Route("api/v1/contract/{ID}/payee")]
        [Route("api/v1/contract/payee/{ID}")]
        [Route("api/v1/contract/{IDs}/backup")]
        [HttpOptions]
        public void Options()
        {
        }

    } 
}
