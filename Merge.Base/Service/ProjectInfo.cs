using Merge.Base.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public class ProjectInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public int Area { get; set; }

        public int Phase { get; set; }

        public int Manager { get; set; }

        public string Note { get; set; }

        public string DisableWord { get; set; }

        public List<ProjectSpecialtyEntity> ProjSpecils { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsDelete { get; set; }

        public ProjectInfo()
        { }

        public ProjectInfo(ProjectEntity Info)
        {
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.Number = Info.Number;
            this.Area = Info.Area;
            this.Phase = Info.Phase;
            this.Manager = Info.Manager;
            this.Note = Info.Note;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
            this.DisableWord = Info.DisableWord;
        }
    }
}
