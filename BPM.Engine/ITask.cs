using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public interface ITask
    {
        Task<string> Excute(ProcessInstance pi);
    }
}
