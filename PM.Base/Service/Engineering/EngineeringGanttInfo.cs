using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class EngineeringGanttInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Parent { get; set; }

        public List<ProcessNodeInfo> Tasks { get; set; }
    }

    /// <summary>
    /// 流程节点信息
    /// </summary>
    public class ProcessNodeInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public DateTime? Est { get; set; }

        public DateTime? Lct { get; set; }

        public string Color { get; set; }

        public int State { get; set; }

        public int User { get; set; }

    }
}
