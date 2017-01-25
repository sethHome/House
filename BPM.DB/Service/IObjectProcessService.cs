using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace BPM.DB
{   
    /// <summary>
    /// ObjectProcess 接口
    /// </summary>
    public interface IObjectProcessService    
    {    
		
		PageSource<ObjectProcessEntity> GetPagedList(PageQueryParam PageParam);

		ObjectProcessEntity Get(int ID);

        ObjectProcessEntity Get(string ObjectKey, int ObjectID);

        ObjectProcessEntity Get(string ProcessID);

        int Add(ObjectProcessEntity ObjectProcess);

		void Update(int ID,ObjectProcessEntity ObjectProcess);

		void Delete(int ID);

        void Delete(string ObjectKey,int ObjectID);

        void Delete(string IDs);
    } 
}
