using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.ProcessModel
{
    /// <summary>
    /// 会签任务
    /// </summary>
    public class JointlySign : BaseTask
    {
        [XmlElement("potentialOwner")]
        public PotentialOwner PotentialOwner { get; set; }
    }
}
