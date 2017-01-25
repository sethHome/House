using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq.Expressions;

namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessTaskIDMap 接口
    /// </summary>
    public partial interface IBPMProcessTaskIDMapService    
    {

        List<BPMProcessTaskIDMapEntity> GetList(Expression<Func<BPMProcessTaskIDMapEntity, bool>> Expression);

		BPMProcessTaskIDMapEntity Get(int ID);

		int Add(BPMProcessTaskIDMapEntity BPMProcessTaskIDMap);

		void Update(int ID,BPMProcessTaskIDMapEntity BPMProcessTaskIDMap);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
