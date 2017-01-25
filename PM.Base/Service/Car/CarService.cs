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
    /// Car 服务
    /// </summary>
    public partial class CarService : ICarService
    {    
		private BaseRepository<CarEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public CarService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<CarEntity>(this._PMContext);
        }

		public PageSource<CarInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<CarEntity, bool>> expression = c => !c.IsDelete;

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
                    
                    default:
                        break;
                }
            }
            #endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<CarInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new CarInfo(entity)
                {
                    
                });
            }

            return new PageSource<CarInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

        public List<CarEntity> GetMyUsedCar(int UserID)
        {
            var users = this._PMContext.CarUseEntity.Where(c => !c.IsDelete && (UserID == 0 || c.Manager == UserID))
                .Select(c => c.CarID);

            var query = this._PMContext.CarEntity.Where(c => users.Contains(c.ID));

            return query.ToList();
        }

        public CarEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(CarInfo Car)
		{
			var entity = new CarEntity(Car);
            
            entity.IsDelete = false;
            entity.Status = (int)CarStatus.正常;
            entity.CreateDate = DateTime.Now;
            this._DB.Add(entity);

            //foreach (var attachID in Car.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
		}

		public void Update(int ID,CarEntity Car)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(Car);

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
                ObjectKey = "Car",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public void ChangeStatus(int ID , CarStatus Status)
        {
            var entity = this._DB.Get(ID);
            entity.Status = (int)Status;
            this._DB.Edit(entity);
        }
        
    } 
}
