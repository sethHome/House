using Api.Framework.Core.BaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BusinessSystem
{
    public class BusinessSystemInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public List<SysUserEntity> Users { get; set; }

        public List<EnumInfo> Enums { get; set; }
    }
}
