using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class AutoCreateArchiveInfo
    {
        public string FondsNumber { get; set; }

        public string FileNumber { get; set; }

        public string ArchiveNode { get; set; }

        public int AccessLevel { get; set; }

        public int UserID { get; set; }

        public string IDs { get; set; }

        public List<Condition> Conditions { get; set; }

        public PageQueryParam Param { get; set; }
    }

    public class CreateArchiveInfo
    {
        public int ArchiveVolumeID { get; set; }

        public string FileIDs { get; set; }

        public string FondsNumber { get; set; }

        public string ArchiveType { get; set; }

        public string FileNumber { get; set; }

        public int AccessLevel { get; set; }

        public int Copies { get; set; }

        public EnumArchiveName ArchiveName { get; set; }

        public EnumArchiveStatus ArchiveStatus { get; set; }

        public string ArchiveNode { get; set; }

        public int UserID { get; set; }

        public int ProjectID { get; set; }

        public string Category { get; set; }

        public List<FieldInfo> Fields { get; set; }

        public List<FieldInfo> ProjectFields { get; set; }
    }
}
