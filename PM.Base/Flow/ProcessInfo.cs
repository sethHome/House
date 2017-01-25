using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class ProcessInfo
    {
        public string ProcessID { get; set; }

        public string CurrentTaskID { get; set; }

        public string CurrentTaskName { get; set; }

        public int CurrentTaskUser { get; set; }

        public int CurrentTaskTime { get; set; }

        public int ProcessStatus { get; set; }
    }
}
