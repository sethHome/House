using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public abstract class IToken
    {
        public TaskInstance CurrentTask { get; set; }

        public GatewayInstance CurrentGateway { get; set; }

        public virtual IToken Copy()
        {
            return null;
        }

        public Dictionary<string, IToken> Split(List<string> targets)
        {
            var result = new Dictionary<string, IToken>();

            targets.ForEach(t =>
            {
                result.Add(t, this.Copy());
            });

            return result;
        }

        public virtual async Task<bool> Excute(ProcessInstance pi)
        {
            var flage = false;

            if (CurrentTask != null)
            {
                var next = await CurrentTask.Excute(pi);

                if ((CurrentTask is ManualTaskInstance || 
                    CurrentTask is JointlySignTaskInstance) && string.IsNullOrEmpty(next))
                {
                    pi.Status = ProcessStatus.Waiting;
                    // 中断
                    flage = true;
                }
                else 
                {
                    CurrentTask.Status = TaskStatus.Done;
                    CurrentTask.ExecuteDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(next))
                    {
                        set(pi, next, this);
                    }
                }
            }
            else if (CurrentGateway != null)
            {
                var result = await CurrentGateway.Excute(pi, this, tos =>
                {

                    if (tos.Count == 1)
                    {
                        set(pi, tos[0], this);
                    }
                    else if (tos.Count > 1)
                    {
                        var spiltTokens = this.Split(tos);

                        // 令牌被拆分，原令牌注销
                        pi.Tokens.Remove(this);

                        tos.ForEach(t =>
                        {
                            set(pi, t, spiltTokens[t]);

                            pi.Tokens.Add(spiltTokens[t]);
                        });


                        flage = true;
                    }
                });


                return flage | result;
            }

            return false;
        }

        private void set(ProcessInstance pi, string to, IToken token)
        {
            token.CurrentTask = null;
            token.CurrentGateway = null;

            if (pi.Tasks.ContainsKey(to))
            {
                pi.Tasks[to].Token = token;
            }
            else if (pi.Gateways.ContainsKey(to))
            {
                pi.Gateways[to].Token = token;
            }
        }
    }
}
