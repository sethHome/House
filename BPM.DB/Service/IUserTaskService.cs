using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BPM.DB
{
    public interface IUserTaskService
    {
        UserTaskEntity Get(int ID);

        UserTaskEntity GetCurrentTask(Guid ProcessID);

        List<UserTaskEntity> GetTaskLog(Guid ProcessID);

        int Add(UserTaskEntity UserTask);

        Guid TaskDone(int TaskID, string Node);

        void Update(int ID, UserTaskEntity UserTask);

        void Delete(int ID);

        void DeleteProcessTask(Guid ProcessID);

        void Delete(string IDs);

        /// <summary>
        /// 重新设置用户任务的接收人
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="ID"></param>
        void ResetTaskUser(Guid ProcessID, List<TaskInfo> TaskUsers);


        int Count(Expression<Func<UserTaskEntity, bool>> predicate);
    }
}
