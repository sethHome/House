using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    /// <summary>
    /// 全宗信息
    /// </summary>
    public class FondInfo
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public List<ArchiveInfo> Archives { get; set; }

        public List<FileLibraryInfo> FileLibrarys { get; set; }

    }
}
