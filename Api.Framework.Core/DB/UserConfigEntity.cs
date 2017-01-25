using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class UserConfigEntity
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public string ConfigName { get; set; }

        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        public string ConfigText { get; set; }
    }
}
