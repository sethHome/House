using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Bid 接口
    /// </summary>
    public partial interface IBidService    
    {    
		
		PageSource<BidInfo> GetPagedList(PageQueryParam PageParam);

		BidEntity Get(int ID);

		int Add(BidInfo Bid);

		void Update(int ID,BidEntity Bid);

        void BackUp(string IDs);

        void Delete(int ID); 

		void Delete(string IDs);
    } 
}
