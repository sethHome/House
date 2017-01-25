using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class UserTaskCountInfo
    {
        /// <summary>
        /// 生产任务数量
        /// </summary>
        public int ProductionCount { get; set; }

        /// <summary>
        /// 表单数量
        /// </summary>
        public int FormCount { get; set; }

        /// <summary>
        /// 提资数量
        /// </summary>
        public int ProvideCount { get; set; }
    }
}
