using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    /// <summary>
    /// 流程的流转信息
    /// </summary>
    public class FlowNodeInfo
    {
        /// <summary>
        /// 当前步骤名称
        /// </summary>
        public string CurrentTaskName { get; set; }

        /// <summary>
        /// 下一步名称
        /// </summary>
        public string NextName { get; set; }

        /// <summary>
        /// 下一步候选人
        /// </summary>
        public List<int> NextUsers { get; set; }

        /// <summary>
        /// 下一步是否会签
        /// </summary>
        public bool NextIsJoinSign { get; set; }

        ///// <summary>
        ///// 会签人员名称
        ///// </summary>
        //public string JoinSignOwner { get; set; }

        /// <summary>
        /// 已经存在的下一步人员
        /// </summary>
        public int NextUser { get; set; }

        /// <summary>
        /// 已经存在的下一步会签人员
        /// </summary>
        public string[] NextJoinSignUsers { get; set; }

        /// <summary>
        /// 下一步人员的变量名称
        /// </summary>
        public string NextOwner { get; set; }

        /// <summary>
        /// 提交需要提供的参数
        /// </summary>
        public List<string> Params { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessID { get; set; }

    }
}
