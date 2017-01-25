using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;

namespace PM.Base.Permission
{
    public class PMPermissionCheck : IPMPermissionCheck
    {
        [Dependency]
        public IPermissionCheck _IPermissionCheck { get; set; }

        private Dictionary<string, PermissionStatus> _CheckCache;

        public PMPermissionCheck()
        {
            _CheckCache = new Dictionary<string, PermissionStatus>();
        }

        public PermissionStatus Check(string PermissionKey, string UserIdentity)
        {
            if (_CheckCache.ContainsKey(PermissionKey + UserIdentity))
            {
                return _CheckCache["PermissionKey + UserIdentity"];
            }
            else
            {
                var result = _IPermissionCheck.Check(PermissionKey, UserIdentity, "System3");
                _CheckCache.Add(PermissionKey + UserIdentity, result);

                return result;
            }
        }

        public PermissionStatus CheckObjAll(int UserID)
        {
            return this.Check("DATA_ObjAll", UserID.ToString());
        }

        public PermissionStatus CheckObjDept(int UserID)
        {
            return this.Check("DATA_ObjDept", UserID.ToString());
        }
    }
}
