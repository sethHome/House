using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveLogEntity
    {
        public int ID { get; set; }

        public string Fonds { get; set; }

        public string ArchiveType { get; set; }

        public int ArchiveID { get; set; }

        public int LogType { get; set; }

        public DateTime LogDate { get; set; }

        public int LogUser { get; set; }

        public string LogContent { get; set; }
    }
}
