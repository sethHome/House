using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using BPM.DB;

namespace PM.Base
{   
    /// <summary>
    /// ObjectProcess 服务
    /// </summary>
    public partial class PMObjectProcessService : IObjectProcessService
    {    
		private BaseRepository<ObjectProcessEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public PMObjectProcessService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<ObjectProcessEntity>(this._PMContext);
        }

		public PageSource<ObjectProcessEntity> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<ObjectProcessEntity, bool>> expression = c => true;

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
			#endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
      
            return new PageSource<ObjectProcessEntity>()
            {
                Source = pageSource.ToList(),
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public ObjectProcessEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

        public ObjectProcessEntity Get(string ProcessID)
        {
            return this._DB.SingleOrDefault(o => o.ProcessID == new Guid(ProcessID) );
        }

        public ObjectProcessEntity Get(string ObjectKey, int ObjectID)
        {
            return this._DB.SingleOrDefault(o => o.ObjectKey == ObjectKey && o.ObjectID == ObjectID);
        }

        public int Add(ObjectProcessEntity ObjectProcess)
		{
            this._DB.Add(ObjectProcess);

            return ObjectProcess.ID;
		}

		public void Update(int ID,ObjectProcessEntity ObjectProcess)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(ObjectProcess);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
			this._DB.Delete(ID);
		}

		public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

        public void Delete(string ObjectKey, int ObjectID)
        {
            this._DB.Delete(o => o.ObjectKey == ObjectKey && o.ObjectID == ObjectID);
        }

        private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "ObjectProcess",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

       
    } 
}
