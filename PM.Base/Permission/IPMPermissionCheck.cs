using Api.Framework.Core.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base.Permission
{
    public interface IPMPermissionCheck
    {
        PermissionStatus Check(string PermissionKey, string UserIdentity);

        PermissionStatus CheckObjAll(int UserID);

        PermissionStatus CheckObjDept(int UserID);
    }
}
