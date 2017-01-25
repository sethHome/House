
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
    /// Bid Controlle
    /// </summary>
    public partial class BidController : ApiController
    {   
		[Dependency]
		public IBidService _IBidService { get; set; }

		[Route("api/v1/bid")]
        [HttpGet]
        public PageSource<BidInfo> All(
			int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
			string orderdirection = "desc",
			string txtfilter = "",
            bool trash = false,
            int bidstatus = 0,
            int depositstatus = 0)
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

            param.FilterCondtion.Add("BidStatus", bidstatus);
            param.FilterCondtion.Add("DepositStatus", depositstatus);
            
            return _IBidService.GetPagedList(param);
        }

		[Route("api/v1/bid/{ID}")]
        [HttpGet]
        public BidEntity One(int ID)
        {
            return this._IBidService.Get(ID);
        }

		[Route("api/v1/bid")]
        [HttpPost]
        public int Create(BidInfo Info)
        {
           return this._IBidService.Add(Info);
        }

		[Route("api/v1/bid/{ID}")]
        [HttpPut]
        public void Update(int ID,BidEntity Entity)
        {
            this._IBidService.Update(ID, Entity);
        }

        [Route("api/v1/bid/{IDs}/backup")]
        [HttpDelete]
        public void BackUp(string IDs)
        {
            this._IBidService.BackUp(IDs);
        }

        [Route("api/v1/bid/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
			if (ID.Contains(","))
            {
                this._IBidService.Delete(ID);
            }
            else
            {
                this._IBidService.Delete(int.Parse(ID));
            }
        }

		[Route("api/v1/bid")]
        [Route("api/v1/bid/{ID}")]
        [Route("api/v1/bid/{IDs}/backup")]
        [HttpOptions]
        public void Option()
        { }
		
    } 
}
