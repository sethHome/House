using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using BPM.ProcessModel;
using System.Linq.Expressions;
using BPM.DB;

namespace PM.Base
{   
    /// <summary>
    /// UserTask 接口
    /// </summary>
    public partial interface IPMUserTaskService : IUserTaskService
    {
        Dictionary<int, int> GetUserTaskCount(int UserID);

        PageSource<UserTaskInfo> GetProductionTasks(PageQueryParam PageParam);

        PageSource<UserTaskInfo> GetProvideTasks(PageQueryParam PageParam);

        PageSource<UserTaskInfo> GetFormTasks(PageQueryParam PageParam);

    } 
}
