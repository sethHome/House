using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class StatisticsInfo
    {
        public int Key { get; set; }

        public int? Count { get; set; }

        public decimal? Money { get; set; }

        public int TotalCount { get; set; }

        public double Percent { get; set; }
    }
}
