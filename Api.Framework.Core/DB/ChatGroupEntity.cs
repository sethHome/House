using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class ChatGroupEntity
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public string GroupDesc { get; set; }

        public bool IsPublic { get; set; }

        public int CreateEmpID { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsDelete { get; set; }
    }
}
