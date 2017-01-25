using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    [Serializable]
    public class PageQueryParam
    {
        public PageQueryParam()
        {
            FilterCondtion = new Hashtable();
        }

        /// <summary>
        /// 是否启用分页
        /// </summary>
        public bool IsAllowPage { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 查询的条件
        /// </summary>
        public Hashtable FilterCondtion { get; set; }

        /// <summary>
        /// 查询文本
        /// </summary>
        public string TextCondtion { get; set; }

        /// <summary>
        /// 是否删除记录
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 排序的ORDER
        /// </summary>
        public string OrderFiled { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public int CurrentUser { get; set; }

        public bool IsDesc { get; set; }

        public Dictionary<string, int> Orders { get; set; }
    }
}
