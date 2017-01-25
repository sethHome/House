using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class EngineeringProductionInfo
    {
        public EngineeringEntity Engineering { get; set; }

        public ProjectEntity Project { get; set; }

        public EngineeringSpecialtyEntity Specialty { get; set; }

        public EngineeringVolumeEntity Volume { get; set; }

        public Guid? ProcessID { get; set; }
    }
}
