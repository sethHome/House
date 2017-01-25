using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public interface IPermissionCheck
    {
        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="UserIdentity">用户标志</param>
        /// <param name="PermissionKey">权限Key</param>
        /// <returns></returns>
        PermissionStatus Check( string PermissionKey, string UserIdentity,string BusinessKey);
        
    }
}
