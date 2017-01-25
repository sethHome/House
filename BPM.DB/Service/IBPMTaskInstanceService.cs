using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq.Expressions;
using BPM.ProcessModel;

namespace BPM.DB
{   
    /// <summary>
    /// BPMTaskInstance 接口
    /// </summary>
    public partial interface IBPMTaskInstanceService    
    {

        List<BPMTaskInstanceEntity> GetList(Expression<Func<BPMTaskInstanceEntity, bool>> Expression);

        BPMTaskInstanceEntity Get(Guid ID);

		string Add(BPMTaskInstanceInfo BPMTaskInstance);

		void Update(string ID,BPMTaskInstanceEntity BPMTaskInstance);

        void UpdateTaskUsers(Guid ProcessID, List<TaskInfo> TaskUsers);

        void Delete(string ID); 

		void Deletes(string IDs);
    } 
}
