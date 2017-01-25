using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;

namespace Api.Framework.Core.Tag
{   
    /// <summary>
    /// SysTag 服务
    /// </summary>
    public partial class SysTagService : ISysTagService
    {    
		private BaseRepository<SysTagEntity> _DB;

        public SysTagService()
        {
            this._DB = new BaseRepository<SysTagEntity>(new SystemContext());
        }

		public PageSource<SysTagEntity> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<SysTagEntity, bool>> expression = c => !c.IsDelete;

            if (!string.IsNullOrEmpty(PageParam.TextCondtion)) {
                expression = expression.And(c => c.TagName.Contains(PageParam.TextCondtion));
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
                    case "ObjectKey":
                        {
                            expression = expression.And(c => c.ObjectKey == val);
                            break;
                        }
                    default:
                        break;
                }
            }

            var source = this._DB.GetPagedList(expression, PageParam);

			return new PageSource<SysTagEntity>(source);
		}

		public SysTagEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public void Add(SysTagEntity SysTag)
		{
			this._DB.Add(SysTag);
		}

		public void Update(int ID,SysTagEntity SysTag)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(SysTag);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
			this._DB.Delete(ID);
		}
		
    } 
}
