using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;
using System.Web.Http;

namespace PM.Base
{   
    /// <summary>
    /// FormChange 接口
    /// </summary>
    public partial interface IFormChangeService    
    {    
		
		PageSource<FormChangeInfo> GetPagedList(PageQueryParam PageParam);

        FormChangeInfo Get(int ID);

        IHttpActionResult Export(int ID);

        Task<int> Add(FormChangeInfo FormChange);

		void Update(int ID,FormChangeEntity FormChange);

		void Delete(int ID); 

		void Delete(string IDs);

        List<BizObject> GetVolumeChanges(int VolumeID);
    } 
}
