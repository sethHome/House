using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    [Serializable]
    public class OB
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
