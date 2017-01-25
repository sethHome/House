using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    [XmlRoot("definitions")]
    public class Definitions
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("process")]
        public Process Process { get; set; }

        [XmlElement("conditionCode")]
        public string ConditionCode { get; set; }

        [XmlElement("resource")]
        public List<Resouce> Resources { get; set; }

        [XmlElement("ob")]
        public OB ProcessOb { get; set; }
    }
}
