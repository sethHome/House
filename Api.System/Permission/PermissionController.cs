using Api.Framework.Core;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System
{

    public class PermissionController : ApiController
    {
        [Dependency]
        public IPermissionPersistence _IPermissionPersistence { get; set; }

        [Dependency]
        public IPermissionCheck _IPermission { get; set; }

        [Token]
        [Route("api/v1/permission/{Key}/check")]
        [HttpGet]
        public PermissionStatus Check(string Key,string user = "",string business="System1")
        {
            if (!string.IsNullOrEmpty(user))
            {
                return _IPermission.Check(Key, user, business);
            }
            else if (!string.IsNullOrEmpty(base.User.Identity.Name))
            {
                return _IPermission.Check(Key, base.User.Identity.Name, business);
            }

            return PermissionStatus.Reject;
        }

        [Route("api/v1/permission")]
        [HttpGet]
        public List<Permission> GetAll(int type = 0,string business = "")
        {
            var filters = new Dictionary<string, object>();
            filters.Add("type", type);
            filters.Add("business", business);

            return _IPermissionPersistence.All(filters);
        }

        [Route("api/v1/permission")]
        [HttpPost]
        public PermissionEntity Create(PermissionEntity Permission)
        {
            _IPermissionPersistence.Create(Permission);

            return Permission;
        }

        [Route("api/v1/permission")]
        [HttpPut]
        public PermissionEntity Update(PermissionEntity Permission)
        {
            _IPermissionPersistence.Update(Permission);

            return Permission;
        }

        [Route("api/v1/permission/{ID}")]
        [HttpDelete]
        public void Delete(int ID)
        {
            _IPermissionPersistence.Delete(ID);
        }

        [Route("api/v1/permission")]
        [Route("api/v1/permission/{ID}")]
        [Route("api/v1/permission/{Key}/check")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
