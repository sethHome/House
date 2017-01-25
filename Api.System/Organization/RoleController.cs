using Api.Framework.Core;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Modules
{

    public class RoleController : ApiController
    {
        [Dependency]
        public IRole _IRole { get; set; }

        [Dependency]
        public IPermissionCheck _IPermission { get; set; }

        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        [Route("api/v1/role")]
        [HttpGet]
        public List<RoleInfo> All()
        {
            return _IRole.All();
        }

        [Route("api/v1/role")]
        [HttpPost]
        public string Create(OrgInfo Org)
        {
            return _IRole.CreateRole(Org.Name);
        }

        [Route("api/v1/role")]
        [HttpPut]
        public void ReName(OrgInfo Org)
        {
            _IRole.ReName(Org.Key, Org.Name);
        }

        [Route("api/v1/role")]
        [HttpDelete]
        public void Delete(string Key = "")
        {
            _IRole.Delete(Key);
        }

        [Route("api/v1/role/{Key}/permision/{BusinessName}")]
        [HttpGet]
        public List<Permission> GetDeptPermission(string BusinessName,string Key)
        {
            var permisson = _IRole.GetRolePermissions(Key);

            return _IPermissionOperate.Prase(BusinessName,permisson);
        }

        [Route("api/v1/role/{Key}/{BusinessKey}/permission")]
        [HttpPut]
        public void SaveDeptPermission(string Key, string BusinessKey, Dictionary<int, long[]> Permissions)
        {
            _IRole.SetPermissions(Key, BusinessKey, Permissions);
        }

        [Route("api/v1/role/{Key}/user")]
        [HttpPut]
        public void SaveDeptUser(string Key, List<SysUserEntity> Users)
        {
            _IRole.SetUsers(Key, Users);
        }

        [Route("api/v1/role")]
        [Route("api/v1/role/{Key}/user")]
        [Route("api/v1/role/{Key}/{BusinessKey}/permission")]
        [Route("api/v1/role/{Key}/permision/{BusinessName}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
