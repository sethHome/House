using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class ParallelGatewayInstance : GatewayInstance
    {
        /// <summary>
        /// 是否是会签网关
        /// </summary>
        public bool IsJointlySignBegin { get; set; }

        public bool IsJointlySignEnd { get; set; }

        public override async Task<bool> Excute(ProcessInstance pi, IToken token, Action<List<string>> callBack)
        {
            //if (this.IsJointlySignBegin)
            //{
            //    pi.GeneratJoinTasks(this);
            //}

            base.ArrivedCount++;

            if (this.Froms.Count == ArrivedCount)
            {
                var newTos = new List<string>();

                foreach (var to in this.Tos)
                {
                    var sto = await pi.Sequences[to].Excute(pi);
                    if (!string.IsNullOrEmpty(sto))
                    {
                        newTos.Add(sto);
                    }
                }

                callBack(newTos);

                return false;
            }
            else
            {
                pi.Tokens.Remove(token);
                token = null;

                return true;
            }
        }
    }
}
