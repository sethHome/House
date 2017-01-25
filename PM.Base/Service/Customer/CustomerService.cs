using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Microsoft.Practices.Unity;

namespace PM.Base
{   
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class CustomerService : ICustomerService
    {    
		private BaseRepository<CustomerEntity> _DB;

        private PMContext _PMContext;

        [Dependency]
        public ICustomerPersonService _ICustomerPersonService { get; set; }

        public CustomerService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<CustomerEntity>(_PMContext);
        }

		public PageSource<CustomerInfo> GetPagedList(PageQueryParam PageParam)
		{
            Expression<Func<CustomerEntity, bool>> expression = c => c.IsDeleted == PageParam.IsDelete;

            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(c => c.Name.Contains(PageParam.TextCondtion) || c.Tel.Contains(PageParam.TextCondtion));
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
                    case "Type":
                        {
                            var id = int.Parse(val);
                            expression = expression.And(c => c.Type == id);
                            break;
                        }
                    case "LevelRate":
                        {
                            var id = int.Parse(val);
                            if (id >= 0)
                            {
                                expression = expression.And(c => c.LevelRate == id);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<CustomerInfo>();

            foreach (var entity in pageSource)
            {
                source.Add(new CustomerInfo(entity)
                {
                    Persons = _PMContext.CustomerPersonEntity.Where(p => p.CustomerID == entity.ID && !p.IsDeleted).ToList()
                });
            }

            return new PageSource<CustomerInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

		public CustomerEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(CustomerInfo Customer)
		{
            var entity = new CustomerEntity(Customer);
            entity.IsDeleted = false;
            
            this._DB.Add(entity);

            if (Customer.Persons != null) {
                Customer.Persons.ForEach(p => {
                    p.CustomerID = entity.ID;
                    _ICustomerPersonService.Add(p);
                });
            }

            return entity.ID;
        }

		public void Update(int ID,CustomerEntity Customer)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(Customer);

			this._DB.Edit(entity);
		}

        public void BackUp(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                var entity = this._DB.Get(int.Parse(id));
                entity.IsDeleted = false;
                this._DB.Edit(entity);
            }
        }

        public void Delete(int ID)
		{
            var entity = this._DB.Get(ID);
            entity.IsDeleted = true;
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

        public List<CustomerEntity> GetSource(string filter)
        {
            Expression<Func<CustomerEntity, bool>> expression = c => !c.IsDeleted;

            if (!string.IsNullOrEmpty(filter))
            {
                expression = expression.And(c => c.Name.Contains(filter) || c.Tel.Contains(filter));
            }

            return this._DB.GetList(expression).ToList();
        }

        
    } 
}
