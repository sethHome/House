using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    public class BaseSequenceFlow
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("sourceRef")]
        public string SourceRef { get; set; }

        [XmlAttribute("targetRef")]
        public string TargetRef { get; set; }
    }
}
