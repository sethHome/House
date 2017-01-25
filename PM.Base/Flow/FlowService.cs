using Api.Framework.Core;
using Api.Framework.Core.Config;
using BPM.DB;
using BPM.Engine;
using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class FlowService
    {
        public static FlowNodeInfo GetInitFlowNodeInfo(string FlowName, Dictionary<string, Object> Params = null)
        {
            var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();

            var def = ProcessModelCache.Instance[FlowName];

            var node = new FlowNodeInfo();

            var list = getFlowTasks(def.Process).Take(2).ToList();

            node.CurrentTaskName = list.First().Name;

            if (list.Count == 2)
            {
                node.NextName = list.Last().Name;
                node.NextIsJoinSign = list.Last().IsJoinSign;
                node.NextOwner = list.Last().Owner;

                // 不是会签才取候选人
                if (!node.NextIsJoinSign)
                {
                    node.NextUsers = new List<int>();

                    var owner = list.Last().Owner;

                    def.Resources.ForEach(r =>
                    {
                        if (r.ID == owner)
                        {
                            if (r.IOwner != null)
                            {
                                var call = UnityContainerHelper.GetServer<IOwner>(r.IOwner.Name);

                                node.NextUsers = call.GetOwner("_" + list[1].DefID, Params);
                            }

                            node.NextUsers = node.NextUsers ?? new List<int>();

                            r.Users.ForEach(u =>
                            {
                                var id = int.Parse(u.ID);
                                if (!node.NextUsers.Contains(id))
                                {
                                    node.NextUsers.Add(id);
                                }
                            });
                        }
                    });
                }
            }

            return node;
        }

        public static FlowNodeInfo GetFlowNodeInfo(int TaskID,string System, Dictionary<string, Object> Params = null)
        {
            var _IUserTaskService = UnityContainerHelper.GetServer<IUserTaskService>(System);
            var _IBPMProcessInstanceService = UnityContainerHelper.GetServer<IBPMProcessInstanceService>();
            var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();

            var userTask = _IUserTaskService.Get(TaskID);
            var process = _IBPMProcessInstanceService.Get(userTask.ProcessID.Value);

            var def = ProcessModelCache.Instance[process.Name];

            var node = new FlowNodeInfo();

            node.ProcessID = userTask.ProcessID.Value.ToString();

            if (!string.IsNullOrEmpty(userTask.Args))
            {
                node.Params = userTask.Args.Split(',').ToList();
            }

            var list = getFlowTasks(def.Process);

            var index = list.FindIndex(u => "_" + u.DefID == userTask.TaskModelID);

            node.CurrentTaskName = list[index].Name;

            if (index < list.Count - 1)
            {
                var nextTask = list[index + 1];
                node.NextName = nextTask.Name;
                node.NextUsers = new List<int>();

                node.NextIsJoinSign = nextTask.IsJoinSign;
                node.NextOwner = nextTask.Owner;

                // 如果下一个任务已执行过，既被退回重新提交
                var tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == userTask.ProcessID
                    && t.SourceID == "_" + nextTask.DefID
                    && !t.IsDelete);

                if (tasks.Count == 1)
                {
                    if (tasks[0].Type == (int)TaskType.Manual)
                    {
                        node.NextUser = tasks[0].UserID;
                    }
                    else
                    {
                        node.NextJoinSignUsers = tasks[0].Users.Split(',');
                    }
                }

                var owner = list[index + 1].Owner;

                def.Resources.ForEach(r =>
                {
                    if (r.ID == owner)
                    {
                        if (r.IOwner != null)
                        {
                            var call = UnityContainerHelper.GetServer<IOwner>(r.IOwner.Name);

                            node.NextUsers = call.GetOwner("_" + nextTask.DefID, Params);
                        }

                        node.NextUsers = node.NextUsers ?? new List<int>();

                        if (!string.IsNullOrEmpty(r.Task))
                        {
                            var orgTask = _IBPMTaskInstanceService.GetList(t => t.ProcessID == userTask.ProcessID
                                && t.SourceID == r.Task
                                && !t.IsDelete);

                            if (orgTask != null && orgTask.Count > 0)
                            {
                                node.NextUsers.Add(orgTask.First().UserID);
                            }
                        }

                        r.Users.ForEach(u =>
                        {
                            var id = int.Parse(u.ID);
                            if (!node.NextUsers.Contains(id))
                            {
                                node.NextUsers.Add(id);
                            }
                        });
                    }
                });
            }

            return node;
        }

        private static List<FlowTask> getFlowTasks(Process process)
        {
            var list = new List<FlowTask>();

            foreach (var task in process.UserTasks)
            {
                list.Add(new FlowTask()
                {
                    DefID = int.Parse(task.ID.Substring(1)),
                    Name = task.Name,
                    Owner = task.PotentialOwner.Name,
                    IsJoinSign = false
                });
            }
            foreach (var task in process.JointlySigns)
            {
                list.Add(new FlowTask()
                {
                    DefID = int.Parse(task.ID.Substring(1)),
                    Name = task.Name,
                    Owner = task.PotentialOwner.Name,
                    IsJoinSign = true
                });
            }

            return list.OrderBy(t => t.DefID).ToList();
        }

        public static ProcessInfo GetFlowInfo(string System,string Key, int ID)
        {
            var _IObjectProcessService = UnityContainerHelper.GetServer<IObjectProcessService>(System);
            var _IUserTaskService = UnityContainerHelper.GetServer<IUserTaskService>(System);
            var _IBPMProcessInstanceService = UnityContainerHelper.GetServer<IBPMProcessInstanceService>();

            var objProcess = _IObjectProcessService.Get(Key, ID);
            var processInfo = _IBPMProcessInstanceService.Get(objProcess.ProcessID);

            var result = new ProcessInfo();
            result.ProcessStatus = processInfo.Status;
            result.ProcessID = processInfo.ID.ToString();
            if (processInfo.Status != (int)ProcessStatus.Finish)
            {
                var userTask = _IUserTaskService.GetCurrentTask(objProcess.ProcessID);

                result.CurrentTaskID = userTask.Identity.ToString();
                result.CurrentTaskName = userTask.Name;
                result.CurrentTaskUser = userTask.UserID;
                result.CurrentTaskTime = userTask.Time;
            }

            return result;
        }

        public static FlowDetailInfo GetFlowDetail(string System,string ProcessID)
        {
            var _IBPMTaskInstanceService = UnityContainerHelper.GetServer<IBPMTaskInstanceService>();

            var _IUserTaskService = UnityContainerHelper.GetServer<IUserTaskService>(System);

            var tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == new Guid(ProcessID)
               && !t.IsDelete && (t.Type == (int)TaskType.Manual || t.Type == (int)TaskType.Sign));


            var taskLog = _IUserTaskService.GetTaskLog(new Guid(ProcessID));

            return new FlowDetailInfo()
            {
                Logs = taskLog,
                Tasks = tasks
            };
        }

        public static FlowDetailInfo GetFlowDetail(string System, string ObjectKey, int ObjectID)
        {
            var _IObjectProcessService = UnityContainerHelper.GetServer<IObjectProcessService>(System);
            var objProcess = _IObjectProcessService.Get(ObjectKey, ObjectID);

            return GetFlowDetail(System,objProcess.ProcessID.ToString());
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetFlowUser()
        {
            var _IBaseConfig = UnityContainerHelper.GetServer<IBaseConfig>();

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith("User.") && c.Key.EndsWith("Tasks"), "User.");

            var result = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();

            nodes.ForEach(user => {
                //  用户
                var users = new Dictionary<string, Dictionary<string, string>>();
                user.ChildNodes[0].ChildNodes.ForEach(spec => {

                    //  专业
                    var specs = new Dictionary<string, string>();
                    spec.ChildNodes[0].ChildNodes.ForEach(pro => {
                        //  流程
                        specs.Add(pro.NodeName, pro.ChildNodes[0].NodeValue);
                    });

                    users.Add(spec.NodeName, specs);

                });

                result.Add(user.NodeName, users);
            });

            return result;
        }
    }
}
