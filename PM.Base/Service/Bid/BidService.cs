using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

namespace PM.Base
{   
    /// <summary>
    /// Bid 服务
    /// </summary>
    public partial class BidService : IBidService
    {    
		private BaseRepository<BidEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public INotificationService _INotificationService { get; set; }

        public BidService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<BidEntity>(this._PMContext);
        }

		public PageSource<BidInfo> GetPagedList(PageQueryParam PageParam)
		{
            Expression<Func<BidEntity, bool>> expression = c => c.IsDelete == PageParam.IsDelete;

			#region Filter
			if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion));
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
                    case "ID":
                        {
                            var id = int.Parse(val);
                            if (id > 0) {
                                expression = expression.And(c => c.ID == id);
                            } 
                            break;
                        }
                    case "BidStatus":
                        {
                            if (val == "false")
                            {
                                expression = expression.And(c => !c.IsDelete);
                            }
                            break;
                        }
                    case "DepositStatus":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.DepositFeeStatus == id);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
			#endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<BidInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new BidInfo(entity)
                {
                    Customer = this._PMContext.CustomerEntity.SingleOrDefault(c => c.ID == entity.CustomerID),
                    Person = this._PMContext.CustomerPersonEntity.SingleOrDefault(c => c.ID == entity.PersonID)
                });
            }

            return new PageSource<BidInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public BidEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(BidInfo Bid)
        {
            var entity = new BidEntity(Bid);

            entity.IsDelete = false;
            this._DB.Add(entity);

            // 创建系统提醒
            CreateOrRemoveNotify(entity);

            return entity.ID;
        }

        /// <summary>
        /// 创建系统提醒
        /// </summary>
        /// <param name="entity"></param>
        private void CreateOrRemoveNotify(BidEntity entity)
        {
            // 如果有应退的担保金，且担保金状态为未退，则创建一个系统提醒
            if (entity.DepositFeeStatus == (int)DepositFeeStatus.未退 &&
                entity.DepositFee - entity.ServiceFee > 0)
            {
                var notifys = _INotificationService.Get("Bid", entity.ID, "ReturnFee");

                var effectNotifys = notifys.Where(n => !n.InvalidDate.HasValue || n.InvalidDate > DateTime.Now);

                // 如果该招投标没有提醒或者所有的提醒已经过期
                if (notifys.Count == 0 ||
                    effectNotifys.Count() == 0)
                {
                    _INotificationService.Add(new NotificationInfo()
                    {
                        CreateDate = DateTime.Now,
                        EffectDate = DateTime.Now, // 生效日期
                        Title = string.Format("招投标项目:{0}的应退担保金：￥{1}未退回", entity.Name, entity.DepositFee - entity.ServiceFee),
                        Info = "",
                        ReceiveUser = entity.Manager,
                        SendUser = 0,
                        SourceID = entity.ID,
                        SourceName = "Bid",
                        SourceTag = "ReturnFee",
                    });
                }
                else if(effectNotifys.Count() == 1)
                {
                    // 如果已经有一个生效的提醒，则更新提醒的接受人和标题
                    var notify = effectNotifys.First();

                    notify.Title = string.Format("招投标项目:{0}的应退担保金：￥{1}未退回", entity.Name, entity.DepositFee - entity.ServiceFee);
                    notify.ReceiveUser = entity.Manager;

                    _INotificationService.Update(notify);
                }
            }
            else
            {
                // 应退的保证金如果已退
                _INotificationService.Delete("Bid", entity.ID, "ReturnFee");
            }
        }

        public void Update(int ID,BidEntity Bid)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(Bid);

            this._DB.Edit(entity);

            // 创建系统提醒
            CreateOrRemoveNotify(Bid);
        }

		public void Delete(int ID)
		{
            var entity = this._DB.Get(ID);

            entity.IsDelete = true;

            this._DB.Edit(entity);

            // 删除对应的招投标提醒
            _INotificationService.Delete("Bid", ID);
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
                ObjectKey = "Bid",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public void BackUp(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                var entity = this._DB.Get(int.Parse(id));
                entity.IsDelete = false;
                this._DB.Edit(entity);
            }
        }
    } 
}
