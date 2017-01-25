using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    public class Resouce
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("user")]
        public List<User> Users { get; set; }

        [XmlElement("IOwner")]
        public CallOwner IOwner { get; set; }

        [XmlElement("task")]
        public string Task { get; set; }
    }
}
