﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    public class ExclusiveGateway : Gateway
    {
        [XmlAttribute("default")]
        public string Default {get;set;}
    }
}
