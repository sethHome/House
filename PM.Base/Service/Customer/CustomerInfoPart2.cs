using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public partial class CustomerInfo
    {
        public int Index { get; set; }

        public List<CustomerPersonEntity> Persons { get; set; }
    }
}
