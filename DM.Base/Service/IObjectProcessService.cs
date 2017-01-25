using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using DM.Base.Entity;

namespace DM.Base.Service
{   
    /// <summary>
    /// ObjectProcess 接口
    /// </summary>
    public partial interface IDMObjectProcessService    
    {    
		ObjectProcessEntity Get(int ID);

        ObjectProcessEntity Get(string ObjectKey, int ObjectID);

        ObjectProcessEntity Get(string ProcessID);

        int Add(ObjectProcessInfo ObjectProcess);

		void Update(int ID,ObjectProcessEntity ObjectProcess);

		void Delete(int ID);

        void Delete(string ObjectKey,int ObjectID);

        void Delete(string IDs);
    } 
}
