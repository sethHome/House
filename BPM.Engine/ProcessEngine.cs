using Api.Framework.Core;
using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    /// <summary>
    /// 流程引擎
    /// </summary>
    public class ProcessEngine
    {
        private static ProcessEngine _Instance;
        private static object _lockObj = new object();

        private ProcessEngine()
        {
            this._ProcessInstances = new Dictionary<string, ProcessInstance>();
            this.modelInstanceIDMaps = new Dictionary<string, string>();
        }

        public static ProcessEngine Instance
        {
            get
            {

                lock (_lockObj)
                {
                    if (_Instance == null)
                    {
                        _Instance = new ProcessEngine();
                    }

                    return _Instance;
                }
            }
        }

        /// <summary>
        /// 流程实例
        /// </summary>
        private Dictionary<string, ProcessInstance> _ProcessInstances;

        private Dictionary<string, string> modelInstanceIDMaps { get; set; }

        //public ProcessEngine()
        //{
        //    this._ProcessInstances = new Dictionary<string, ProcessInstance>();
        //    this.modelInstanceIDMaps = new Dictionary<string, string>();
        //}

        public string CreateProcessInstance(string Identity, int User, Dictionary<string, object> InputData = null)
        {
            // 获取流程模板
            var def = ProcessModelCache.Instance[Identity];

            // 获取流程的监听者
            var _Ob = UnityContainerHelper.GetServer<IObservation>(def.ProcessOb.Name);

            var pID = Guid.NewGuid().ToString();

            // 生成流程实例
            var pi = new ProcessInstance()
            {
                ID = pID,
                Name = def.ID,
            };

            pi.StartDate = DateTime.Now;
            pi.Version = 1;
            pi.CreateUser = User;
            pi.InputData = InputData;

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

            // 编译脚本代码
            if (!string.IsNullOrEmpty(def.ConditionCode))
            {
                pi.Compiled = ConditionExpression.Evaluator(def.ConditionCode, def.ID);
            }

            // 生成流程脚本实例
            pi.GeneratTasks(def, InputData, _Ob);

            // 缓存流程实例
            _ProcessInstances.Add(pID, pi);

            // 保存生成的流程实例
            BPMDBService.Create(pi);

            return pID;
        }

        public void AddUser(string Pid, string RsKey, string UserID, string UserName)
        {
            var pi = _ProcessInstances[Pid];
            pi.ProcessResouces[RsKey].Users.Add(UserID, UserName);
        }

        public async Task<bool> Start(string DefinitionID, Dictionary<string, object> InputData = null)
        {
            var token = new TaskToken();
            var pi = _ProcessInstances[DefinitionID];
            pi.StartTask.Token = token;
            pi.Tokens.Add(token);
            pi.Status = ProcessStatus.Running;

            if (InputData != null)
            {
                pi.InputData = InputData;
            }
            if (pi.InputData == null)
            {
                pi.InputData = new Dictionary<string, object>();
            }

            while (pi.Status == ProcessStatus.Running)
            {
                foreach (var t in pi.Tokens)
                {
                    if (await t.Excute(pi))
                    {
                        //  令牌数量发生变化，集合更改，重新循环
                        break;
                    }
                }
            }

            if (pi.Status == ProcessStatus.Finish)
            {
                // 异步更新状态
                BPMDBService.ProcessDone(pi.ID);

                // 从缓存中移除已完成的流程实例
                _ProcessInstances.Remove(pi.ID);

                // 通知业务系统流程完成
                pi.OB.ProcessFinish(pi);
            }

            return true;
        }

        public async Task Continu(string TaskID, int User, Dictionary<string, object> InputData = null)
        {
            TaskID = TaskID.ToUpper();

            var pid = getProcessInstance(TaskID);

            ProcessInstance pi = null;

            if (string.IsNullOrEmpty(pid))
            {
                // back from db
                pi = BPMDBService.BackUpInstance(TaskID);

                // 缓存流程实例
                if (!_ProcessInstances.ContainsKey(pi.ID))
                {
                    _ProcessInstances.Add(pi.ID, pi);
                }
            }
            else
            {
                pi = _ProcessInstances[pid];
            }

            pi.Status = ProcessStatus.Running;

            pi.InputData = InputData;

            // 这里不自动跳过此任务，由任务自己来执行下一步（因为会签的话需要所有的会签任务完成才执行下一步）

            //var token = pi.Tasks[TaskID].Token;

            //token.CurrentTask = null;
            //token.CurrentGateway = null;

            //var to = pi.Sequences[pi.Tasks[TaskID].To].To;

            //if (pi.Tasks.ContainsKey(to))
            //{
            //    pi.Tasks[to].Token = token;
            //}
            //else if (pi.Gateways.ContainsKey(to))
            //{
            //    pi.Gateways[to].Token = token;
            //}

            pi.Tasks[TaskID].IsDone = true;

            while (pi.Status == ProcessStatus.Running)
            {
                foreach (var t in pi.Tokens)
                {
                    if (await t.Excute(pi))
                    {
                        //  令牌数量发生变化，集合更改，重新循环
                        break;
                    }
                }
            }

            if (pi.Status == ProcessStatus.Finish)
            {
                // 更新状态
                BPMDBService.ProcessDone(pi.ID);

                // 从缓存中移除已完成的流程实例
                _ProcessInstances.Remove(pi.ID);

                // 通知业务系统流程完成
                pi.OB.ProcessFinish(pi);
            }
        }

        /// <summary>
        /// 删除流程实例
        /// </summary>
        /// <param name="Pid"></param>
        public bool DeleteProcess(string Pid)
        {
            // 删除流程实例的存储数据
            if (BPMDBService.DeleteProcess(Pid))
            {
                if (_ProcessInstances.ContainsKey(Pid))
                {
                    _ProcessInstances.Remove(Pid);
                }

                return true;
            }


            return false;
        }

        /// <summary>
        /// 设置流程任务的用户
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="Data"></param>
        public void SetProceeTaskUsers(Guid Pid, List<TaskInfo> TaskUsers)
        {
            var strPid = Pid.ToString("N");

            // 更新内存中的流程实例用户
            if (_ProcessInstances.ContainsKey(strPid))
            {
                var process = _ProcessInstances[strPid];

                foreach (var task in TaskUsers)
                {
                    var taskInstanceID = process.ModelInstanceIDMaps[task.ID];

                    process.Tasks[taskInstanceID].UserID = task.User;
                }
            }

            // 更新存储中的用户ID
            BPMDBService.UpdateTaskUsers(Pid, TaskUsers);
        }


        private string getProcessInstance(string taskID)
        {
            foreach (var item in _ProcessInstances)
            {
                foreach (var task in item.Value.Tasks)
                {
                    if (task.Key == taskID)
                    {
                        return item.Key;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 运行时给流程实例添加参数
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void SetProcessInputData(string processID, string Key, object Value)
        {
            var process = this._ProcessInstances[processID];

            if (process.InputData.ContainsKey(Key))
            {
                process.InputData[Key] = Value;
            }
            else
            {
                process.InputData.Add(Key, Value);
            }
        }

        private string generatIds(string id)
        {
            if (this.modelInstanceIDMaps.ContainsKey(id))
            {
                return this.modelInstanceIDMaps[id];
            }
            else
            {
                var guid = Guid.NewGuid().ToString("N");
                this.modelInstanceIDMaps.Add(id, guid);

                return guid;
            }

        }
    }
}
