﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    public class SequenceFlow
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("sourceRef")]
        public string SourceRef { get; set; }

        [XmlAttribute("targetRef")]
        public string TargetRef { get; set; }

        [XmlElement("conditionExpression")]
        public string Condition { get; set; }
    }
}
