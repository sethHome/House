using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BPM.DB;
using Api.Framework.Core;
using BPM.ProcessModel;

namespace BPM.Engine
{
    public class BPMDBService
    {
        /// <summary>
        /// 流程实例持久化
        /// </summary>
        /// <param name="Process"></param>
        public static void Create(ProcessInstance Process)
        {
            //var _IBPMProcessTaskIDMapService = UnityContainerHelper.GetServer<IBPMProcessTaskIDMapService>();
            //var _IBPMProcessInstanceService = UnityContainerHelper.GetServer<IBPMProcessInstanceService>();
            //var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();

            var _IBPMProcessTaskIDMapService = new BPMProcessTaskIDMapService();
            var _IBPMProcessInstanceService = new BPMProcessInstanceService();
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            _IBPMProcessInstanceService.Add(new BPMProcessInstanceEntity()
            {
                ID = new Guid(Process.ID),
                CreateUser = Process.CreateUser,
                Name = Process.Name,
                Status = (int)Process.Status,
                Version = Process.Version,
                StartDate = Process.StartDate,
            });

            foreach (var item in Process.ModelInstanceIDMaps)
            {
                _IBPMProcessTaskIDMapService.Add(new BPMProcessTaskIDMapEntity()
                {
                    ProcessID = new Guid(Process.ID),
                    TaskID = new Guid(item.Value),
                    TaskKey = item.Key
                });
            }

            foreach (var item in Process.Tasks)
            {
                var task = new BPMTaskInstanceInfo()
                {
                    ID = new Guid(item.Key),
                    ProcessID = new Guid(Process.ID),
                    Name = item.Value.Name,
                    Status = (int)item.Value.Status,
                    Type = (int)item.Value.Type,
                    UserID = item.Value.UserID,
                    SourceID = item.Value.SourceID
                };

                if (item.Value.From != null)
                {
                    task.Source = new Guid(item.Value.From);
                }
                if (item.Value.To != null)
                {
                    task.Target = new Guid(item.Value.To);
                }

                _IBPMTaskInstanceService.Add(task);
            }
        }

        /// <summary>
        /// 恢复流程实例
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public static ProcessInstance BackUpInstance(string TaskID)
        {
            //var _IBPMProcessTaskIDMapService = UnityContainerHelper.GetServer<IBPMProcessTaskIDMapService>();
            //var _IBPMProcessInstanceService = UnityContainerHelper.GetServer<IBPMProcessInstanceService>();
            //var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();

            var _IBPMProcessTaskIDMapService = new BPMProcessTaskIDMapService();
            var _IBPMProcessInstanceService = new BPMProcessInstanceService();
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            var task = _IBPMTaskInstanceService.Get(new Guid(TaskID));
            var tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == task.ProcessID);
            var process = _IBPMProcessInstanceService.Get(task.ProcessID);
            var idMaps = _IBPMProcessTaskIDMapService.GetList(m => m.ProcessID == task.ProcessID);

            // 获取流程模板
            var def = ProcessModelCache.Instance[process.Name];

            // 获取流程的监听者
            var _Ob = UnityContainerHelper.GetServer<IObservation>(def.ProcessOb.Name);


            // 生成流程实例
            var pi = new ProcessInstance()
            {
                ID = task.ProcessID.ToString(),
                Name = def.Name,
                StartDate = process.StartDate,
                Version = process.Version
            };

            // 编译脚本代码
            if (!string.IsNullOrEmpty(def.ConditionCode))
            {
                pi.Compiled = ConditionExpression.Evaluator(def.ConditionCode, def.ID);
            }

            // 流程资源
            def.Resources.ForEach(r =>
            {
                var prs = new ProcessResouce()
                {
                    Key = r.ID,
                    Users = new Dictionary<string, string>()
                };

                r.Users.ForEach(u =>
                {
                    prs.Users.Add(u.ID, u.Name);
                });

                pi.ProcessResouces.Add(r.ID, prs);
            });

            pi.ModelInstanceIDMaps = new Dictionary<string, string>();

            idMaps.ForEach(m =>
            {
                pi.ModelInstanceIDMaps.Add(m.TaskKey, m.TaskID.ToString());
            });

            pi.OB = _Ob;

            // 生成流程脚本实例
            pi.BackUpTasks(def, tasks);

            return pi;
        }

        public static void TaskTurn(string ID, int UserID, string Users = "")
        {
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            _IBPMTaskInstanceService.TaskTurn(ID, UserID, Users);
        }

        public static void TaskDone(string ID, int UserID)
        {
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            _IBPMTaskInstanceService.TaskDone(ID, UserID);
        }

        public static void ProcessDone(string ProcessID)
        {
            var _BPMProcessInstanceService = new BPMProcessInstanceService();

            _BPMProcessInstanceService.ProcessFinish(ProcessID);
        }

        public static bool DeleteProcess(string ProcessID)
        {
            var _BPMProcessInstanceService = new BPMProcessInstanceService();
            return _BPMProcessInstanceService.Delete(ProcessID);
        }

        public static void UpdateTaskUsers(Guid ProcessID, List<TaskInfo> TaskUsers)
        {
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            _IBPMTaskInstanceService.UpdateTaskUsers(ProcessID, TaskUsers);
        }

        /// <summary>
        /// 设置任务的计划时间
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        public static void SetTaskDate(string ProcessID, DateTime? BeginDate, DateTime? EndDate)
        {
            if (!BeginDate.HasValue || !EndDate.HasValue)
            {
                return;
            }

            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            var tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == new Guid(ProcessID)
                && !t.IsDelete && t.Type == (int)TaskType.Manual);

            tasks.First().EstDate = BeginDate;
            tasks.Last().LctDate = EndDate;

            var dayDiff = EndDate.Value - BeginDate.Value;

            var fistTaskDuration = 0.8;
            var taskDuration = 0.2 / (tasks.Count - 1);

            for (int i = 0; i < tasks.Count; i++)
            {
                if (i == 0)
                {
                    tasks[i].EstDate = BeginDate;
                    tasks[i].LctDate = BeginDate.Value.AddMinutes(dayDiff.TotalMinutes * fistTaskDuration); // 第一个时间占80%
                }
                else
                {
                    tasks[i].EstDate = tasks[i - 1].LctDate.Value.AddMinutes(15); // 每个任务之间的间隔是15分钟

                    if (i != tasks.Count - 1)
                    {
                        tasks[i].LctDate = tasks[i].EstDate.Value.AddMinutes(dayDiff.TotalMinutes * taskDuration); // 其他任务的时间是20%平分
                    }
                    else
                    {
                        // 最后一个任务的结束时间就是整个流程的结束时间
                        tasks[i].LctDate = EndDate;
                    }
                }

                // 更新
                _IBPMTaskInstanceService.SetTask(tasks[i]);
            }
        }

        public static List<BPMTaskInstanceEntity> GetProcessTasks(Guid ProcessID)
        {
            var _IBPMTaskInstanceService = new BPMTaskInstanceService();

            return _IBPMTaskInstanceService.GetList(t => t.ProcessID == ProcessID
                && !t.IsDelete && t.Type == (int)TaskType.Manual).OrderBy(t => int.Parse(t.SourceID.Substring(1))).ToList();
        }

        public static Dictionary<int, int> CreateJoinSignTasks(string TaskID, string Users)
        {
            var _BPMJoinSignTaskService = new BPMJoinSignTaskService();

           return _BPMJoinSignTaskService.CreateTasks(TaskID, Users);
        }

        public static bool CheckAllJoinSignTaskDone(string TaskID)
        {
            var _BPMJoinSignTaskService = new BPMJoinSignTaskService();

            return _BPMJoinSignTaskService.CheckAllTaskDone(TaskID);
        }
        public static bool CheckFailureSign(string TaskID)
        {
            var _BPMJoinSignTaskService = new BPMJoinSignTaskService();

            return _BPMJoinSignTaskService.CheckFailureSign(TaskID);
        }
        
        public static bool ExistsJoinSignTask(string TaskID)
        {
            var _BPMJoinSignTaskService = new BPMJoinSignTaskService();

            return _BPMJoinSignTaskService.Exists(TaskID);
        }

        public static void JoinSignTaskDone(int TaskID, bool Result)
        {
            var _BPMJoinSignTaskService = new BPMJoinSignTaskService();

            _BPMJoinSignTaskService.TaskDone(TaskID, Result);
        }

    }
}
