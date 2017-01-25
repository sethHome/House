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

namespace Api.System.Organization
{

    public class DepartmentController : ApiController
    {
        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public IPermissionCheck _IPermission { get; set; }

        [Dependency]
        public IPermissionOperate _IPermissionOperate { get; set; }

        [Route("api/v1/department")]
        [HttpGet]
        public List<DepartmentInfo> All()
        {
            return _IDepartment.All();
        }

        [Route("api/v1/department")]
        [HttpPost]
        public string CreateDepartment(OrgInfo Org)
        {
            return _IDepartment.AddDepartment(Org.Name, Org.Key);
        }

        [Route("api/v1/department")]
        [HttpPut]
        public void ReName(OrgInfo Org)
        {
            _IDepartment.ReName(Org.Key, Org.Name);
        }

        [Route("api/v1/department")]
        [HttpDelete]
        public void DeleteDepartment(string Key = "")
        {
            _IDepartment.Delete(Key);
        }

        [Route("api/v1/department/{DeptKey}/permission/{BusinessName}")]
        [HttpGet]
        public List<Permission> GetDeptPermission(string DeptKey, string BusinessName, string inherit = "true")
        {
            return _IDepartment.GetDeptPermissions(BusinessName, DeptKey, inherit == "true");
        }

        [Route("api/v1/department/{DeptKey}/{BusinessKey}/permission")]
        [HttpPut]
        public void SaveDeptPermission(string DeptKey, string BusinessKey, DepartmentPermission Permissions)
        {
            _IDepartment.SetPermissions(DeptKey, BusinessKey, Permissions.Permissions);

            _IDepartment.SetUnInheritPermissions(DeptKey, BusinessKey,Permissions.UnInheritPermissions);
        }

        [Route("api/v1/department/{DeptKey}/user")]
        [HttpPut]
        public void SaveDeptUser(string DeptKey, List<SysUserEntity> Users)
        {
            _IDepartment.SetUsers(DeptKey, Users);
        }

        [Route("api/v1/department")]
        [Route("api/v1/department/{DeptKey}/user")]
        [Route("api/v1/department/{DeptKey}/{BusinessKey}/permission")]
        [Route("api/v1/department/{DeptKey}/permission/{BusinessName}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
