using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{ 
    public enum ArchiveNodeType
    {
        /// <summary>
        /// 全宗节点
        /// </summary>
        Fonds = 0,
        /// <summary>
        /// 档案节点
        /// </summary>
        Archive = 1,
        /// <summary>
        /// 分类节点
        /// </summary>
        Category = 2,
        /// <summary>
        /// 数据节点
        /// </summary>
        DataFilter = 3,
    }
}
