using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class AutoTaskInstance : TaskInstance
    {
        private IAutoTaskExcute _IAutoTaskExcute;

        public AutoTaskInstance(IAutoTaskExcute ex)
        {
            _IAutoTaskExcute = ex;
        }

        public override async Task<string> Excute(ProcessInstance pi)
        {
            // 第一次轮到
            var arg = getArg(pi);

            var datas = _IAutoTaskExcute.Excute(arg);

            if (datas != null)
            {
                foreach (var item in datas)
                {
                    if (!pi.InputData.ContainsKey(item.Key))
                    {
                        pi.InputData.Add(item.Key, item.Value);
                    }

                }
            }

            return await base.Excute(pi);
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
                arg.TaskUser = "";
                arg.IsBack = true;
            }
          
            return arg;
        }

    }
}
