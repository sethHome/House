using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class ProcessResouce
    {
        public string Key { get; set; }

        public Dictionary<string, string> Users { get; set; }

        public Dictionary<string, Dictionary<string, string>> UserGroups { get; set; }
    }
}
