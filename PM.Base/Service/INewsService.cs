using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// News 接口
    /// </summary>
    public partial interface INewsService    
    {    
		
		PageSource<NewsInfo> GetPagedList(PageQueryParam PageParam);

		NewsEntity Get(int ID);

		int Add(NewsInfo News);

		void Update(int ID,NewsEntity News);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
