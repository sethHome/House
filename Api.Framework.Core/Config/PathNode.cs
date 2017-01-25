using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Config
{
    public class PathNode
    {
        #region main

        /// <summary>
        /// 路径中被分隔符分割后的一段路径
        /// </summary>
        public string PartPath { get; set; }
        /// <summary>
        /// 被分割的某段路径之前的路径片段
        /// </summary>
        public string BeforePartPath { get; set; }
        /// <summary>
        /// 路径片段在完整路径中的深度
        /// </summary>
        public int Deep { get; set; }
        #endregion

        #region extension

        public string Value { get; set; }

        public string Tag { get; set; }

        public string Type { get; set; }

        public int ID { get; set; }

        #endregion
    }
}
