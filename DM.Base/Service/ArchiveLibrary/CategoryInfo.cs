using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class CategoryInfo
    {
        public string Parent { get; set; }

        public List<CategoryInfo> Children { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }
    }
}
