using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace Api.Framework.Core.Tag
{   
    /// <summary>
    /// ObjectTag 接口
    /// </summary>
    public partial interface IObjectTagService    
    {    
		
		PageSource<ObjectTagEntity> GetPagedList(PageQueryParam PageParam);

        List<SysTagEntity> GetObjectTags(string ObjectKey, int ObjectID);

		ObjectTagEntity Get(int ID);

		void Add(string Key, int ID, List<ObjectTagInfo> Tags);

		void Update(int ID,ObjectTagEntity ObjectTag);

		void Delete(int ID); 
    } 
}
