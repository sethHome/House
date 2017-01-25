using Api.Framework.Core.DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Chat
{
    public class ChatService
    {
        public static PageSource<ChatMessageEntity> GetMessage(int PageIndex,int PageSize,string user, string TargetGroup,string TargetUser)
        {
            var _Context = new SystemContext();

            Expression<Func<ChatMessageEntity, bool>> expression = c => true;

            if (!string.IsNullOrEmpty(TargetGroup))
            {
                expression = expression.And(m => m.TargetGroup == TargetGroup);
            }
            else if (!string.IsNullOrEmpty(TargetUser))
            {
                expression = expression.And(m => (m.TargetUser == user && m.UserIdentity == TargetUser) || (m.TargetUser == TargetUser && m.UserIdentity == user));
            }

            var result = _Context.ChatMessageEntity.Where(expression)
                .OrderByDescending(t => t.Date)
                .ToPagedList(PageIndex, PageSize);

            return new PageSource<ChatMessageEntity>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public static int CreateChatGroup(ChatGroupInfo GroupInfo)
        {
            var _Context = new SystemContext();

            var entity = new ChatGroupEntity()
            {
                GroupName = GroupInfo.GroupName,
                GroupDesc = GroupInfo.GroupDesc,
                CreateDate = DateTime.Now,
                CreateEmpID = GroupInfo.CreateEmpID,
                IsDelete = false,
                IsPublic = false
            };

            _Context.ChatGroupEntity.Add(entity);
            _Context.SaveChanges();

            // 将自己添加进组成员
            _Context.ChatGroupEmpsEntity.Add(new ChatGroupEmpsEntity()
            {
                GroupID = entity.GroupID,
                EmpID = GroupInfo.CreateEmpID
            });

            foreach (var id in GroupInfo.Emps)
            {
                if (id != GroupInfo.CreateEmpID)
                {
                    _Context.ChatGroupEmpsEntity.Add(new ChatGroupEmpsEntity()
                    {
                        GroupID = entity.GroupID,
                        EmpID = id
                    });
                }
            }

            _Context.SaveChanges();

            var notifySrv = UnityContainerHelper.GetServer<WSHandler>();
            notifySrv.Send(new
            {
                TargetGroup = entity.GroupID,
                MessageType = 104
            });

            return entity.GroupID;
        }

        public static void UpdateChatGroup(int GroupID, ChatGroupInfo GroupInfo)
        {
            var _Context = new SystemContext();

            var entity = _Context.ChatGroupEntity.Find(GroupID);

            entity.GroupName = GroupInfo.GroupName;
            entity.GroupDesc = GroupInfo.GroupDesc;

            _Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

            // 先清空小组成员
            var items = _Context.ChatGroupEmpsEntity.Where(g => g.GroupID == GroupID);

            foreach (var item in items)
            {
                _Context.ChatGroupEmpsEntity.Remove(item);
            }

            // 重新生成小组成员
            _Context.ChatGroupEmpsEntity.Add(new ChatGroupEmpsEntity()
            {
                GroupID = entity.GroupID,
                EmpID = entity.CreateEmpID
            });

            foreach (var id in GroupInfo.Emps)
            {
                if (id != GroupInfo.CreateEmpID)
                {
                    _Context.ChatGroupEmpsEntity.Add(new ChatGroupEmpsEntity()
                    {
                        GroupID = entity.GroupID,
                        EmpID = id
                    });
                }
            }

            _Context.SaveChanges();
            var notifySrv = UnityContainerHelper.GetServer<WSHandler>();
            notifySrv.Send(new
            {
                TargetGroup = entity.GroupID,
                MessageType = 104
            });

        }

        public static void RemoveChatGroup(int GroupID)
        {
            var _Context = new SystemContext();

            var entity = _Context.ChatGroupEntity.Find(GroupID);

            entity.IsDelete = true;

            _Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _Context.SaveChanges();

            var notifySrv = UnityContainerHelper.GetServer<WSHandler>();
            notifySrv.Send(new
            {
                TargetGroup = entity.GroupID,
                MessageType = 105
            });
        }

        public static void ExitChatGroup(int GroupID,int UserID)
        {
            var _Context = new SystemContext();

            var entity = _Context.ChatGroupEmpsEntity.SingleOrDefault(g => g.GroupID == GroupID && g.EmpID == UserID);
            _Context.ChatGroupEmpsEntity.Remove(entity);

            _Context.SaveChanges();
            var notifySrv = UnityContainerHelper.GetServer<WSHandler>();
            notifySrv.Send(new
            {
                TargetGroup = entity.GroupID,
                TargetUser = UserID,
                MessageType = 106
            });
        }
    }

}
