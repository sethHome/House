using BPM.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class FlowDetailInfo
    {
        public List<BPMTaskInstanceEntity> Tasks { get; set; }

        public List<UserTaskEntity> Logs { get; set; }
    }
}
