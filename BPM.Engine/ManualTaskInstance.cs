using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class ManualTaskInstance : TaskInstance, ISubject
    {
        private IObservation _Observation;

        public PotentialOwner Owner { get; set; }

        public ManualTaskInstance()
        {
            
        }

        public void SetObservation(IObservation ob)
        {
            _Observation = ob;
        }

        public override async Task<string> Excute(ProcessInstance pi)
        {
            // 第一次轮到
            var arg = getArg(pi);

            if (this.Status == TaskStatus.Waiting)
            {
                // 用户提交了任务，
                this.Status = TaskStatus.Done;
                this.IsDone = true;

                BPMDBService.TaskDone(this.ID, this.UserID);

                // 通知任务完成
                _Observation.TaskDone(arg);

                return await base.Excute(pi);
            }
            else
            {
                this.Status = TaskStatus.Waiting;
                this.TurnDate = DateTime.Now;
                this.UserID = int.Parse(arg.TaskUser);

                // 异步更新任务状态
                BPMDBService.TaskTurn(this.ID, this.UserID);

                if (!await _Observation.TaskTurnCall(arg))
                {
                    // 发起一个用户通知
                    // 中断等待用户回应

                    return string.Empty;
                }
                else
                {
                    // 继续任务
                    return await base.Excute(pi);
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
                arg.TaskUser = this.UserID.ToString();
                arg.IsBack = true;
            }
            else if (this.Owner != null && !string.IsNullOrEmpty(this.Owner.Name))
            {
                if (this.UserID > 0)
                {
                    arg.TaskUser = this.UserID.ToString();
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
