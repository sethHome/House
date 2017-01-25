﻿using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class JointlySignTaskInstance : TaskInstance, ISubject
    {
        private IObservation _Observation;

        /// <summary>
        /// 会签人员
        /// </summary>
        public string Users { get; set; }

        public PotentialOwner Owner { get; set; }

        public void SetObservation(IObservation ob)
        {
            _Observation = ob;
        }

        public override async Task<string> Excute(ProcessInstance pi)
        {
            var arg = getArg(pi);

            // 第一次轮到会签
            if (!BPMDBService.ExistsJoinSignTask(this.ID))
            {
                // 中断等待用户回应
                this.Status = TaskStatus.Waiting;
                this.TurnDate = DateTime.Now;

                // 异步更新任务状态
                BPMDBService.TaskTurn(this.ID, 0, arg.TaskUser);

                // 创建会签人员列表 // TODO  arg.TaskUser == 0
                arg.JoinSigns = BPMDBService.CreateJoinSignTasks(this.ID, arg.TaskUser);

                // 发起一个用户通知
                if (!await _Observation.TaskTurnCall(arg))
                {
                    return string.Empty;
                }
                else
                {
                    // 任务完成
                    BPMDBService.TaskDone(this.ID, int.Parse(arg.TaskUser));

                    // 通知任务完成
                    _Observation.TaskDone(arg);

                    // 继续任务
                    return await base.Excute(pi);
                }
            }
            else
            {
                // 会签任务完成
                BPMDBService.JoinSignTaskDone(int.Parse(pi.InputData["SignID"].ToString()), bool.Parse(pi.InputData["SignIDResult"].ToString()));

                // 通知任务完成
                _Observation.TaskDone(arg);

                // 第二次，检查所有的会签任务是否完成
                if (BPMDBService.CheckAllJoinSignTaskDone(this.ID))
                {
                    
                    // 任务完成
                    BPMDBService.TaskDone(this.ID, 0);

                    // 如果会签的下一个节点是唯一网关，只要有一个会签失败，这个网关的参数设置为false
                    if (BPMDBService.CheckFailureSign(this.ID) && 
                        !string.IsNullOrEmpty(arg.ArgName) && 
                        arg.ArgName.StartsWith("bool_")) {
                        pi.InputData[arg.ArgName] = false;
                    }

                    // 继续任务
                    return await base.Excute(pi);
                }
                else
                {
                    // 没有全部完成 继续等待
                    return string.Empty;
                }
            }
        }

        public BPMObArg getArg(ProcessInstance pi)
        {
            var arg = new BPMObArg()
            {
                ProcessID = pi.ID,
                TaskID = this.ID,
                TaskName = this.Name,
                TaskUser = "0",
                TaskModelID = this.SourceID,
                CreateUser = pi.CreateUser,
                ProcessData = pi.InputData,
            };

            var nextTo = pi.Sequences[this.To].To;

            if (pi.Gateways.ContainsKey(nextTo) &&
                pi.Gateways[nextTo].Type == GatewayType.Exclusive)
            {
                arg.ArgName = pi.Sequences[pi.Gateways[nextTo].Default].Condition;
            }

            if (IsDone)
            {
                arg.TaskUser = this.Users;
                arg.IsBack = true;
            }
            else if (this.Owner != null && !string.IsNullOrEmpty(this.Owner.Name))
            {
                if (!string.IsNullOrEmpty(this.Users))
                {
                    arg.TaskUser = this.Users;
                }
                else if (pi.InputData.ContainsKey(this.Owner.Name))
                {
                    arg.TaskUser = pi.InputData[this.Owner.Name].ToString();
                }
                else if (pi.ProcessResouces[this.Owner.Name].Users.Count > 0)
                {
                    arg.TaskUser = pi.ProcessResouces[this.Owner.Name].Users.First().Key;
                }
            }

            return arg;
        }
    }
}