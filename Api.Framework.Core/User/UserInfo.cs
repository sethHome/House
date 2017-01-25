using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public class UserInfo 
    {
        public int ID { get; set; }

        public string Account { get; set; }

        public string Name { get; set; }

        public string PhotoImg { get; set; }

        public string PhotoImgLarge { get; set; }

        public bool Visiable { get; set; }

        public OrgInfo Dept { get; set; }

        public List<OrgInfo> Roles { get; set; }

        public Dictionary<int, string> PermissionsValue { get; set; }

        public Dictionary<int, string> DeptPermissionsValue { get; set; }

        public Dictionary<int, string> RolePermissionsValue { get; set; }

        public List<UserProductionInfo> ProductionInfos { get; set; }

        public List<BusinessSystemInfo> Businesses { get; set; }
    }
}
