using Api.Framework.Core.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Permission
{
    public interface IDMPermissionCheck
    {
        PermissionStatus Check(string PermissionKey, string UserIdentity);

        int GetUserAccessLevel(int UserID);
    }
}
