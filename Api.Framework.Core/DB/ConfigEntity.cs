using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class ConfigEntity
    {
        public ConfigEntity()
        {
            IsDeleted = false;
        }

        public int ID { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public string Tag { get; set; }

        public bool IsDeleted { get; set; }
    }
}
