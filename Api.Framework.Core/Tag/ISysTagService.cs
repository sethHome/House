using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace Api.Framework.Core.Tag
{   
    /// <summary>
    /// SysTag 接口
    /// </summary>
    public partial interface ISysTagService    
    {    
		
		PageSource<SysTagEntity> GetPagedList(PageQueryParam PageParam);

		SysTagEntity Get(int ID);

		void Add(SysTagEntity SysTag);

		void Update(int ID,SysTagEntity SysTag);

		void Delete(int ID); 
    } 
}
