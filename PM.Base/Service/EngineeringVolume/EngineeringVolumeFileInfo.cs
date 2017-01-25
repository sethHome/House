using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class EngineeringVolumeFileInfo
    { 
        public EngineeringVolumeEntity Volume { get; set; }

        public List<SysAttachFileEntity> Files { get; set; }
    }
}
