using Api.Framework.Core;
using BPM.DB;
using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public class ProcessInstance
    {
        public List<ITask> CurrentTasks { get; set; }

        public Dictionary<string, TaskInstance> Tasks
        {
            get; set;
        }

        public Dictionary<string, GatewayInstance> Gateways
        {
            get; set;
        }

        public Dictionary<string, SequenceInstance> Sequences
        {
            get; set;
        }

        public Dictionary<string, object> InputData { get; set; }

        public object Compiled { get; set; }

        public List<IToken> Tokens { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public int Version { get; set; }

        public int CreateUser { get; set; }

        public ProcessStatus Status { get; set; }

        public StartTaskInstance StartTask { get; set; }

        public Dictionary<string, string> ModelInstanceIDMaps { get; set; }

        public IObservation OB { get; set; }
        

        public Dictionary<string, ProcessResouce> ProcessResouces { get; set; }

        public ProcessInstance()
        {
            this.ModelInstanceIDMaps = new Dictionary<string, string>();
            this.Tasks = new Dictionary<string, TaskInstance>();
            this.CurrentTasks = new List<ITask>();
            this.Gateways = new Dictionary<string, GatewayInstance>();
            this.Sequences = new Dictionary<string, SequenceInstance>();
            this.Tokens = new List<IToken>();
            this.ProcessResouces = new Dictionary<string, ProcessResouce>();
        }

        public void BackUpTasks(Definitions def, List<BPMTaskInstanceEntity> DBTasks)
        {
            #region Tasks

            foreach (var task in DBTasks)
            {
                TaskInstance taskInstance = null;
                var taskID = task.ID.ToString().ToUpper();

                switch (task.Type)
                {
                    case (int)TaskType.Start:
                        {
                            taskInstance = new StartTaskInstance()
                            {
                                Name = task.Name,
                                ID = taskID,
                                SourceID = task.SourceID,
                                From = task.Source.ToString().ToUpper(),
                                To = task.Target.ToString().ToUpper(),
                                Status = (TaskStatus)task.Status,
                                Type = TaskType.Start
                            };
                            break;
                        }
                    case (int)TaskType.CallApi:
                        {
                            taskInstance = new AutoTaskInstance(UnityContainerHelper.GetServer<IAutoTaskExcute>(task.Name))
                            {
                                Name = task.Name,
                                ID = taskID,
                                SourceID = task.SourceID,
                                From = task.Source.ToString().ToUpper(),
                                To = task.Target.ToString().ToUpper(),
                                Status = (TaskStatus)task.Status,
                                Type = TaskType.CallApi
                            };

                            break;
                        }
                    case (int)TaskType.Manual:
                        {
                            var mTask = new ManualTaskInstance()
                            {
                                Name = task.Name,
                                ID = taskID,
                                SourceID = task.SourceID,
                                From = task.Source.ToString().ToUpper(),
                                To = task.Target.ToString().ToUpper(),
                                Status = (TaskStatus)task.Status,
                                Type = TaskType.Manual,
                                UserID = task.UserID
                            };

                            mTask.IsDone = mTask.Status == TaskStatus.Done;

                            foreach (var item in this.ModelInstanceIDMaps)
                            {
                                if (item.Value.ToUpper() == taskID)
                                {
                                    mTask.Owner = def.Process.UserTasks.SingleOrDefault(t => t.ID == item.Key).PotentialOwner;
                                    break;
                                }
                            }

                            if (this.OB != null)
                            {
                                mTask.SetObservation(this.OB);
                            }

                            taskInstance = mTask;
                            break;
                        }
                    case (int)TaskType.Sign:
                        {
                            var mTask = new JointlySignTaskInstance()
                            {
                                Name = task.Name,
                                ID = taskID,
                                SourceID = task.SourceID,
                                From = task.Source.ToString().ToUpper(),
                                To = task.Target.ToString().ToUpper(),
                                Status = (TaskStatus)task.Status,
                                Type = TaskType.Manual,
                                UserID = task.UserID,
                                Users = task.Users
                            };

                            mTask.IsDone = mTask.Status == TaskStatus.Done;

                            foreach (var item in this.ModelInstanceIDMaps)
                            {
                                if (item.Value.ToUpper() == taskID)
                                {
                                    mTask.Owner = def.Process.JointlySigns.SingleOrDefault(t => t.ID == item.Key).PotentialOwner;
                                    break;
                                }
                            }

                            if (this.OB != null)
                            {
                                mTask.SetObservation(this.OB);
                            }

                            taskInstance = mTask;
                            break;
                        }
                    case (int)TaskType.Empty:
                        {
                            break;
                        }
                    case (int)TaskType.End:
                        {
                            taskInstance = new EndTaskInstance()
                            {
                                Name = task.Name,
                                ID = taskID,
                                SourceID = task.SourceID,
                                From = task.Source.ToString().ToUpper(),
                                To = task.Target.ToString().ToUpper(),
                                Status = (TaskStatus)task.Status,
                                Type = TaskType.End
                            };
                            break;
                        }
                }

                if (taskInstance.Status == TaskStatus.Waiting)
                {
                    var token = new TaskToken();
                    taskInstance.Token = token;
                    this.Tokens.Add(token);
                }

                this.Tasks.Add(taskID, taskInstance);
            }
            #endregion

            // 连接线
            def.Process.SequenceFlows.ForEach(s =>
            {
                var taskID = generatIds(s.ID);
                var fromID = generatIds(s.SourceRef);
                var toID = generatIds(s.TargetRef);

                this.Sequences.Add(taskID, new SequenceInstance()
                {
                    Name = s.ID,
                    ID = taskID,
                    To = toID,
                    From = fromID,
                    Condition = s.Condition
                });
            });

            // 独立网关
            def.Process.ExclusiveGateways.ForEach(g =>
            {
                var gid = generatIds(g.ID);

                this.Gateways.Add(gid, new ExclusiveGatewayInstance()
                {
                    ID = gid,
                    Froms = generatIds(g.Incoming),
                    Tos = generatIds(g.Outgoing),
                    Default = generatIds(g.Default),
                    Type = GatewayType.Exclusive
                });
            });

            // 并行网关
            def.Process.ParallelGateways.ForEach(g =>
            {
                var gid = generatIds(g.ID);

                this.Gateways.Add(gid, new ParallelGatewayInstance()
                {
                    ID = gid,
                    Froms = generatIds(g.Incoming),
                    Tos = generatIds(g.Outgoing),
                    Type = GatewayType.Parallel,
                    IsJointlySignBegin = g.Type != null && g.Type.Equals("JointlySignBegin"),
                    IsJointlySignEnd = g.Type != null && g.Type.Equals("JointlySignEnd")
                });
            });
        }

        public void GeneratTasks(Definitions def, Dictionary<string, object> InputData = null, IObservation Ob = null)
        {
            this.OB = Ob;

            var startID = generatIds(def.Process.StartEvent.ID);

            this.StartTask = new StartTaskInstance()
            {
                Name = def.Process.StartEvent.Name,
                SourceID = def.Process.StartEvent.ID,
                ID = startID,
                To = generatIds(def.Process.StartEvent.OutGoing),
                Status = TaskStatus.Empty,
                Type = TaskType.Start
            };

            // 开始节点
            this.Tasks.Add(startID, this.StartTask);

            // 用户手工任务
            def.Process.UserTasks.ForEach(t =>
            {
                var taskID = generatIds(t.ID);
                var fromID = generatIds(t.Incoming);
                var toID = generatIds(t.OutGoing);

                var mTask = new ManualTaskInstance()
                {
                    Name = t.Name,
                    ID = taskID,
                    SourceID = t.ID,
                    To = toID,
                    From = fromID,
                    Status = TaskStatus.Empty,
                    Type = TaskType.Manual,
                    Owner = t.PotentialOwner
                };

                if (mTask.Owner != null && !string.IsNullOrEmpty(mTask.Owner.Name))
                {
                    if (mTask.Owner.Name == "ProcessUser")
                    {
                        mTask.UserID = this.CreateUser;
                    }
                    else if (InputData.ContainsKey(mTask.Owner.Name))
                    {
                        mTask.UserID = int.Parse(InputData[mTask.Owner.Name].ToString());
                    }
                    else if (this.ProcessResouces.ContainsKey(mTask.Owner.Name) && this.ProcessResouces[mTask.Owner.Name].Users.Count == 1)
                    {
                        mTask.UserID = int.Parse(this.ProcessResouces[mTask.Owner.Name].Users.First().Key);
                    }
                }

                if (Ob != null)
                {
                    mTask.SetObservation(Ob);
                }

                this.Tasks.Add(taskID, mTask);
            });

            // 自动调用Api任务
            def.Process.AutoTasks.ForEach(t =>
            {
                var taskID = generatIds(t.ID);
                var fromID = generatIds(t.Incoming);
                var toID = generatIds(t.OutGoing);

                this.Tasks.Add(taskID, new AutoTaskInstance(UnityContainerHelper.GetServer<IAutoTaskExcute>(t.Name))
                {
                    Name = t.Name,
                    ID = taskID,
                    SourceID = t.ID,
                    To = toID,
                    From = fromID,
                    Status = TaskStatus.Empty,
                    Type = TaskType.CallApi
                });
            });

            // 会签任务
            def.Process.JointlySigns.ForEach(t =>
            {
                var taskID = generatIds(t.ID);
                var fromID = generatIds(t.Incoming);
                var toID = generatIds(t.OutGoing);

                var mTask = new JointlySignTaskInstance()
                {
                    Name = t.Name,
                    ID = taskID,
                    SourceID = t.ID,
                    To = toID,
                    From = fromID,
                    Status = TaskStatus.Empty,
                    Type = TaskType.Sign,
                    Owner = t.PotentialOwner
                };

                if (mTask.Owner != null && !string.IsNullOrEmpty(mTask.Owner.Name))
                {
                    if (InputData.ContainsKey(mTask.Owner.Name))
                    {
                        mTask.Users = InputData[mTask.Owner.Name].ToString();
                    }
                    else if (this.ProcessResouces.ContainsKey(mTask.Owner.Name) && this.ProcessResouces[mTask.Owner.Name].Users.Count == 1)
                    {
                        mTask.Users = this.ProcessResouces[mTask.Owner.Name].Users.First().Key;
                    }
                }

                if (Ob != null)
                {
                    mTask.SetObservation(Ob);
                }

                this.Tasks.Add(taskID, mTask);
            });

            // 连接线
            def.Process.SequenceFlows.ForEach(s =>
            {
                var taskID = generatIds(s.ID);
                var fromID = generatIds(s.SourceRef);
                var toID = generatIds(s.TargetRef);

                this.Sequences.Add(taskID, new SequenceInstance()
                {
                    Name = s.ID,
                    ID = taskID,
                    To = toID,
                    From = fromID,
                    Condition = s.Condition
                });
            });

            // 独立网关
            def.Process.ExclusiveGateways.ForEach(g =>
            {
                var gid = generatIds(g.ID);

                this.Gateways.Add(gid, new ExclusiveGatewayInstance()
                {
                    ID = gid,
                    Froms = generatIds(g.Incoming),
                    Tos = generatIds(g.Outgoing),
                    Type = GatewayType.Exclusive,
                    Default = generatIds(g.Default)
                });
            });

            // 并行网关
            def.Process.ParallelGateways.ForEach(g =>
            {
                var gid = generatIds(g.ID);

                this.Gateways.Add(gid, new ParallelGatewayInstance()
                {
                    ID = gid,
                    Froms = generatIds(g.Incoming),
                    Tos = generatIds(g.Outgoing),
                    Type = GatewayType.Parallel,
                    IsJointlySignBegin = g.Type != null && g.Type.Equals("JointlySignBegin"),
                    IsJointlySignEnd = g.Type != null && g.Type.Equals("JointlySignEnd")
                });
            });

            var endId = generatIds(def.Process.EndEvent.ID);
            var endFromID = generatIds(def.Process.EndEvent.Incoming);

            // 结束任务
            this.Tasks.Add(endId, new EndTaskInstance()
            {
                Name = def.Process.EndEvent.Name,
                ID = endId,
                SourceID = def.Process.EndEvent.ID,
                From = endFromID,
                Status = TaskStatus.Empty,
                Type = TaskType.End
            });
        }

        private string generatIds(string id)
        {
            if (this.ModelInstanceIDMaps.ContainsKey(id))
            {
                return this.ModelInstanceIDMaps[id].ToUpper();
            }
            else
            {
                var guid = Guid.NewGuid().ToString("N");
                this.ModelInstanceIDMaps.Add(id, guid);

                return guid.ToUpper();
            }

        }

        private List<string> generatIds(List<string> ids)
        {
            var result = new List<string>();

            foreach (var id in ids)
            {
                result.Add(generatIds(id));
            }

            return result;
        }
    }
}
