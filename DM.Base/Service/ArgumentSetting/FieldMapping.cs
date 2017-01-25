using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class FieldMapping
    {
        public string FondsNumber { get; set; }

        public string ArchiveNumber { get; set; }

        public string ArchiveKey { get; set; }

        public string ArchiveFieldID { get; set; }

        public string FileNumber { get; set; }

        public string FileFieldID { get; set; }

        public int MappingType { get; set; }

        public bool IsRemove { get; set; }
    }
}
