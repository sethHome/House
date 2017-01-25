using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    public class BaseEvent
    {
        [XmlAttribute("id")]
        public string ID { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("outgoing")]
        public string OutGoing { get; set; }

        [XmlElement("incoming")]
        public string Incoming { get; set; }
    }
}
