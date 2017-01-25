using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using Api.Framework.Core.Chat;

namespace PM.Base
{   
    /// <summary>
    /// Notification 服务
    /// </summary>
    public partial class NotificationService : INotificationService
    {    
		private BaseRepository<NotificationEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        public NotificationService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<NotificationEntity>(this._PMContext);
        }

		public PageSource<NotificationInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<NotificationEntity, bool>> expression = c => !c.IsDelete;

			#region Filter
			if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                //expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    case "User":
                        {
                            var id = int.Parse(val);
                            if (id > 0) {
                                expression = expression.And(c => c.ReceiveUser == id);
                            }
                            break;
                        }
                    case "Effect":
                        {
                            var id = int.Parse(val);
                            if (id > 0) {
                                expression = expression.And(c => c.EffectDate <= DateTime.Now && (!c.InvalidDate.HasValue || c.InvalidDate > DateTime.Now));
                            }
                            break;
                        }
                    case "Read":
                        {
                            var id = int.Parse(val);
                            if (id == 1)
                            {
                                expression = expression.And(c => c.IsRead);
                            }
                            else if (id == 2)
                            {
                                expression = expression.And(c => !c.IsRead);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
			#endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<NotificationInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new NotificationInfo(entity)
                {
                    
                });
            }

            return new PageSource<NotificationInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public NotificationEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(NotificationInfo Notification)
		{
			var entity = new NotificationEntity(Notification);
            
            entity.IsDelete = false;
            this._DB.Add(entity);
           
            _NotifySrv.Send(new {
                Target = Notification.ReceiveUser,
                Head = Notification.Head,
                Title = entity.Title,
                Content = entity.Info,
            });

            //foreach (var attachID in Notification.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
		}

		public void Update(NotificationEntity Notification)
		{
			var entity = this._DB.Get(Notification.ID);

			entity.SetEntity(Notification);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
            var entity = this._DB.Get(ID);

            entity.IsDelete = true;

            this._DB.Edit(entity);
        }

		public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "Notification",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public void Delete(string SourceName, int SourceID, string SourceTag)
        {
            this._DB.Delete(n => n.SourceID == SourceID && n.SourceName == SourceName && n.SourceTag == SourceTag);
        }

        public void Delete(string SourceName, int SourceID)
        {
            this._DB.Delete(n => n.SourceID == SourceID && n.SourceName == SourceName );
        }

        public IList<NotificationEntity> Get(string SourceName, int SourceID, string SourceTag)
        {
            var list = this._DB.GetList(n => !n.IsDelete &&
                n.SourceName == SourceName && n.SourceID == SourceID && n.SourceTag == SourceTag);

            return list.ToList();
        }

        /// <summary>
        /// 设置提醒已读
        /// </summary>
        /// <param name="ID"></param>
        public void Read(int ID)
        {
            var entity = this._DB.Get(ID);

            entity.IsRead = true;

            this._DB.Edit(entity);
        }
    } 
}
