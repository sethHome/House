using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public class MergeResult
    {
        public SysAttachFileEntity Attach { get; set; }

        public List<string> DisableWordMatchs { get; set; }

        public List<string> ParaIndexCheck { get; set; }

        public bool ContainsArea { get; set; }

        public Dictionary<string,int> OtherAreas { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}
