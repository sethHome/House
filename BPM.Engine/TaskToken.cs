using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class TaskToken : IToken
    {
        public override IToken Copy()
        {
            return new TaskToken();
        }
    }
}
