using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public enum PermissionStatus
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// 拒绝
        /// </summary>
        Reject = 1,
        /// <summary>
        /// 授予
        /// </summary>
        Confer = 2,
        /// <summary>
        /// 继承
        /// </summary>
        InheritDept = 3,
        /// <summary>
        /// 继承
        /// </summary>
        InheritRole = 4,
        
    }
}
