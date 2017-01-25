using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    [KnownType(typeof(DepartmentInfo))]
    [KnownType(typeof(RoleInfo))]
    public class OrgInfo
    {
        public int ID { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }
    }
}
