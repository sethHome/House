using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    /// <summary>
    /// 独立网关，Tos只能有一个出口
    /// </summary>
    public class ExclusiveGatewayInstance : GatewayInstance
    {
        public override async Task<bool> Excute(ProcessInstance pi, IToken token, Action<List<string>> callBack)
        {
            // 本来就只有一个出口，那么就等待所有的入口到达再出发
            if (this.Tos.Count == 1)
            {
                base.ArrivedCount++;

                if (this.Froms.Count == ArrivedCount)
                {
                    callBack(new List<string>() { pi.Sequences[this.Tos[0]].To });

                    return false;
                }
                else
                {
                    pi.Tokens.Remove(token);
                    token = null;

                    return true;
                }
            }
            else if (this.Froms.Count == 1)
            {
                // 一个入口，多个出口
                // 独立网关需要计算每个出口的表达式
               
                foreach (var to in this.Tos)
                {
                    var sTo = await pi.Sequences[to].Excute(pi);
                    if (sTo != null) {

                        callBack(new List<string>() { sTo });

                        break;
                    }
                }
               
            }

            return false;

        }
    }
}
