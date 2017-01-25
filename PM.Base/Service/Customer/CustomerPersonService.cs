using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;

namespace PM.Base
{   
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial class CustomerPersonService : ICustomerPersonService
    {    
		private BaseRepository<CustomerPersonEntity> _DB;

        public CustomerPersonService()
        {
            this._DB = new BaseRepository<CustomerPersonEntity>(new PMContext());
        }

		public PageSource<CustomerPersonEntity> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<CustomerPersonEntity, bool>> expression = c => true;

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    //case "ID":
                    //    {
                    //        var id = int.Parse(val);
                    //        expression = expression.And(c => c.ID == id);
                    //        break;
                    //    }
                    default:
                        break;
                }
            }

            var source = this._DB.GetPagedList(expression, PageParam);

			return new PageSource<CustomerPersonEntity>(source);
		}

		public CustomerPersonEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public void Add(CustomerPersonEntity CustomerPerson)
		{
			this._DB.Add(CustomerPerson);
		}

		public void Update(int ID,CustomerPersonEntity CustomerPerson)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(CustomerPerson);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
            var entity = this._DB.Get(ID);

            entity.IsDeleted = true;

            this._DB.Edit(entity);
        }
		
    } 
}
