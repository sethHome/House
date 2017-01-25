using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq.Expressions;
using DM.Base.Entity;
using BPM.DB;

namespace DM.Base.Service
{   
    /// <summary>
    /// UserTask 接口
    /// </summary>
    public partial interface IDMUserTaskService  : IUserTaskService  
    {
        PageSource<UserTaskInfo> GetArchiveTasks(PageQueryParam PageParam);
    } 
}
