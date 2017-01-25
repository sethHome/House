using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class VolumeStatisticsInfo
    {
        /// <summary>
        /// 卷册文件数量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 会签人数
        /// </summary>
        public int MuiltySignCount { get; set; }

        /// <summary>
        /// 历时的天数
        /// </summary>
        public int DuringDay { get; set; }

        public List<VolumeCheckGroupInfo> VolumeCheckGroups { get; set; }

    }
}
