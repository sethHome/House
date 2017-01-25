using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public enum TaskType : int
    {
        Start = 0,
        Empty = 1,
        CallApi = 2,
        Manual = 3,
        Sign = 4,
        End = 10
    }

    public enum GatewayType : int
    {
        Exclusive = 1,
        Parallel = 2
    }

    public enum ProcessStatus : int
    {
        Start = 1,
        Running = 2,
        Waiting = 3,
        Finish = 4
    }

    public enum TaskStatus : int
    {
        Empty = 0,
        Waiting = 1,
        Done = 2,
    }
}
