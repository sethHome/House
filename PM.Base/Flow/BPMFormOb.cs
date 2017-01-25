using BPM.DB;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base.Flow
{
    public class BPMFormOb : IObservation
    {
        [Dependency("System3")]
        public IUserTaskService _IUserTaskService { get; set; }
        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        public virtual void ProcessFinish(ProcessInstance pi)
        {
            
        }

        public virtual async Task<bool> TaskTurnCall(BPMObArg arg)
        {
            var userTask = new UserTaskEntity()
            {
                Identity = new Guid(arg.TaskID),
                ProcessID = new Guid(arg.ProcessID),
                Name = arg.TaskName,
                TaskModelID = arg.TaskModelID,
                ReceiveDate = DateTime.Now,
                Source = (int)TaskSource.表单,
                Status = (int)TaskStatus.下达,
                Type = 1,
                //UserID = int.Parse(arg.TaskUser),
                Args = arg.ArgName,
                Time = 1
            };

            if (arg.TaskModelID == "_3" && !arg.IsBack)
            {
                // 流程第一次启动
                //userTask.Status = (int)TaskStatus.完成;
                //userTask.FinishDate = DateTime.Now;

                userTask.UserID = arg.CreateUser;
                
                var id = _IUserTaskService.Add(userTask);
                arg.ProcessData.Add("userTaskID", id);

                // 自动执行这个任务
                await ProcessEngine.Instance.Continu(arg.TaskID, arg.CreateUser, arg.ProcessData);

                return true;
            }
            else
            {
                if (arg.IsBack)
                {
                   var existsTaskCount = _IUserTaskService.Count(t => t.ProcessID == userTask.ProcessID && t.TaskModelID == userTask.TaskModelID && !t.IsDelete);

                    userTask.Time = existsTaskCount + 1;
                }

                if (arg.JoinSigns != null)
                {
                   
                    foreach (var item in arg.JoinSigns)
                    {
                        userTask.UserID = item.Value;
                        userTask.Args = "SignID#" + item.Key.ToString()+","+ arg.ArgName;

                        _IUserTaskService.Add(userTask);
                    }
                }
                else
                {
                    userTask.UserID = int.Parse(arg.TaskUser);
                    _IUserTaskService.Add(userTask);
                }

                return false;
            }
        }

        public virtual void TaskDone(BPMObArg TaskArg)
        {
            var note = TaskArg.ProcessData.ContainsKey("note") ? TaskArg.ProcessData["note"].ToString() : "";
            var taskID = TaskArg.ProcessData.ContainsKey("userTaskID") ? TaskArg.ProcessData["userTaskID"].ToString() : "";

            _IUserTaskService.TaskDone(int.Parse(taskID), note);
        }
    }
}
