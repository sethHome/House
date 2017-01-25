using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class FileLibraryInfo
    {
        public string ID { get; set; }

        public string FondsNumber { get; set; }

        public string ParentFullKey { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// 0 : Fonds, 1 : FileNode, 2 : CategoryNode
        /// </summary>
        public int NodeType { get; set; }

        public List<FileLibraryInfo> Children { get; set; }
    }
}
