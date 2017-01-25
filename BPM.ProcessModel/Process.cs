using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    public class Process
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("processType")]
        public string ProcessType { get; set; }

        [XmlAttribute("isExecutable")]
        public bool IsExecutable { get; set; }

        [XmlAttribute("isClosed")]
        public bool isClosed { get; set; }

        [XmlElement("startEvent")]
        public StartEvent StartEvent { get; set; }

        [XmlElement("endEvent")]
        public EndEvent EndEvent { get; set; }

        [XmlElement("userTask")]
        public List<UserTask> UserTasks { get; set; }

        [XmlElement("autoTask")]
        public List<AutoTask> AutoTasks { get; set; }

        [XmlElement("jointlySign")]
        public List<JointlySign> JointlySigns { get; set; }

        [XmlElement("sequenceFlow")]
        public List<SequenceFlow> SequenceFlows { get; set; }

        [XmlElement("exclusiveGateway")]
        public List<ExclusiveGateway> ExclusiveGateways { get; set; }

        [XmlElement("parallelGateway")]
        public List<ParallelGateway> ParallelGateways { get; set; }
    }
}
