using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessInstance 接口
    /// </summary>
    public partial interface IBPMProcessInstanceService    
    {    
		
		PageSource<BPMProcessInstanceInfo> GetPagedList(PageQueryParam PageParam);

		BPMProcessInstanceEntity Get(Guid ID);

        string Add(BPMProcessInstanceEntity BPMProcessInstance);

		void Update(string ID,BPMProcessInstanceEntity BPMProcessInstance);

        bool Delete(string ID); 

		void Deletes(string IDs);
    } 
}
