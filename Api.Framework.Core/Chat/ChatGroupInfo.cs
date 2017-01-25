using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Chat
{
    public class ChatGroupInfo
    {
        public int GroupID { get; set; }

        public int CreateEmpID { get; set; }

        public string GroupName { get; set; }

        public string GroupDesc { get; set; }

        public int[] Emps { get; set; }
    }
}
