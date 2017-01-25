using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.ProcessModel
{
    public class ProcessModelInfo
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public List<TaskInfo> Tasks { get; set; }
    }

    public class TaskInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public int User { get; set; }
    }
}
