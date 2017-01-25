using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    public class Gateway
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("incoming")]
        public List<string> Incoming { get; set; }

        [XmlElement("outgoing")]
        public List<string> Outgoing { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}
