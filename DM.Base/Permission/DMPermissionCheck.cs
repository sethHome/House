using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;

namespace DM.Base.Permission
{
    public class DMPermissionCheck : IDMPermissionCheck
    {
        [Dependency]
        public IPermissionCheck _IPermissionCheck { get; set; }

        private Dictionary<string, PermissionStatus> _CheckCache;

        public DMPermissionCheck()
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
                var result = _IPermissionCheck.Check(PermissionKey, UserIdentity, "System2");
                _CheckCache.Add(PermissionKey + UserIdentity, result);

                return result;
            }
        }

        public int GetUserAccessLevel(int UserID)
        {
            if (this.Check("DATA-AccessLevel5", UserID.ToString()) != PermissionStatus.Reject)
            {
                return 5;
            }
            else if (this.Check("DATA-AccessLevel4", UserID.ToString()) != PermissionStatus.Reject)
            {
                return 4;
            }
            else if (this.Check("DATA-AccessLevel3", UserID.ToString()) != PermissionStatus.Reject)
            {
                return 3;
            }
            else if (this.Check("DATA-AccessLevel2", UserID.ToString()) != PermissionStatus.Reject)
            {
                return 2;
            }

            return 1;
        }
    }
}
