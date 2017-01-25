using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
   public class DocNode
    {
        public Node Node { get; set; }

        public bool IsListNode { get; set; }

        public int ListValue { get; set; }
    }
}
