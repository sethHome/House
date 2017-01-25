using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class GatewayInstance 
    {
        public string ID { get; set; }

        public int ArrivedCount;

        public List<string> Froms { get; set; }

        public List<string> Tos { get; set; }

        public string ArgName { get; set; }

        public string Default { get; set; }

        public GatewayType Type { get; set; }

        private IToken _Token;
        public IToken Token
        {
            get
            {
                return this._Token;
            }
            set
            {
                this._Token = value;
                value.CurrentGateway = this;
            }
        }

        public GatewayInstance()
        {
            ArrivedCount = 0;
        }


        public virtual async Task<bool> Excute(ProcessInstance pi, IToken token, Action<List<string>> callBack)
        {
            return await Task<bool>.Factory.StartNew(() =>
            {
                return false;
            });
                
        }
    }
}
