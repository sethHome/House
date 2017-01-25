using Api.Framework.Core;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.Chat;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Chat
{
    public class ChatController : ApiController
    {
        [Route("api/v1/message")]
        [HttpGet]
        public PageSource<ChatMessageEntity> All(int pagesize = 50, int pageindex = 1, string targetgroup = "", string targetuser = "", string user = "")
        {
            return ChatService.GetMessage(pageindex, pagesize, user, targetgroup, targetuser);
        }

        [Token]
        [Route("api/v1/group")]
        [HttpPost]
        public int CreateChatGroup(ChatGroupInfo GroupInfo)
        {
            GroupInfo.CreateEmpID = int.Parse(base.User.Identity.Name);

            return ChatService.CreateChatGroup(GroupInfo);
        }

        [Route("api/v1/group/{GroupID}")]
        [HttpPut]
        public void UpdateChatGroup(int GroupID, ChatGroupInfo GroupInfo)
        {
            ChatService.UpdateChatGroup(GroupID, GroupInfo);
        }

        [Route("api/v1/group/{GroupID}")]
        [HttpDelete]
        public void RemoveChatGroup(int GroupID)
        {
            ChatService.RemoveChatGroup(GroupID);
        }

        [Token]
        [Route("api/v1/group/{GroupID}/exit")]
        [HttpDelete]
        public void ExitChatGroup(int GroupID)
        {
            ChatService.ExitChatGroup(GroupID, int.Parse(base.User.Identity.Name));
        }

        [Route("api/v1/message")]
        [Route("api/v1/group")]
        [Route("api/v1/group/{GroupID}")]
        [Route("api/v1/group/{GroupID}/exit")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
