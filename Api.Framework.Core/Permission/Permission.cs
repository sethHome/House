using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public class Permission
    {
        public int ID { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public long Value { get; set; }

        public string StrValue { get; set; }

        public int Index { get; set; }

        public bool IsInherit { get; set; }

        public bool CanInherit { get; set; }

        public bool OrgCanInherit { get; set; }

        public string BusinessName { get; set; }

        public List<Permission> Children { get; set; }
    }
}
