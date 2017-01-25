using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class ChatMessageEntity
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string TargetGroup { get; set; }

        public string TargetUser { get; set; }

        public string UserIdentity { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public int MessageType { get; set; }

        public bool IsReceive { get; set; }
    }
}
