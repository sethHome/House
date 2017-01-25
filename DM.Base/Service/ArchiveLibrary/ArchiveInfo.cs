using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArchiveInfo
    {
        public string FondsNumber { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public bool HasProject { get; set; }

        public bool HasVolume { get; set; }

        public bool HasCategory { get; set; }

        public bool Disabled { get; set; }

        public List<FieldInfo> FileFields { get; set; }

        public List<FieldInfo> ProjectFields { get; set; }

        public List<FieldInfo> VolumeFields { get; set; }

        public List<FieldInfo> BoxFields { get; set; }

        public List<CategoryInfo> Categorys { get; set; }
    }
}
