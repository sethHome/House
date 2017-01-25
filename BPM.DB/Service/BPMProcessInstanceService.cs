using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessInstance 服务
    /// </summary>
    public partial class BPMProcessInstanceService : IBPMProcessInstanceService
    {    
		private BaseRepository<BPMProcessInstanceEntity> _DB;
		private BPMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }
        [Dependency]
        public IBPMProcessTaskIDMapService _IBPMProcessTaskIDMapService { get; set; }
        [Dependency]
        public IBPMTaskInstanceService _IBPMTaskInstanceService { get; set; }

        public BPMProcessInstanceService()
        {
			this._PMContext = new BPMContext();
            this._DB = new BaseRepository<BPMProcessInstanceEntity>(this._PMContext);
        }

		public PageSource<BPMProcessInstanceInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<BPMProcessInstanceEntity, bool>> expression = c => true;

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
            var source = new List<BPMProcessInstanceInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new BPMProcessInstanceInfo(entity)
                {
                    
                });
            }

            return new PageSource<BPMProcessInstanceInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public BPMProcessInstanceEntity Get(Guid ID)
		{
			return this._DB.Get(ID);
		}

		public string Add(BPMProcessInstanceEntity BPMProcessInstance)
		{
			//var entity = new BPMProcessInstanceEntity(BPMProcessInstance);
            
            //entity.IsDelete = false;
            this._DB.Add(BPMProcessInstance);

            //foreach (var attachID in BPMProcessInstance.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return BPMProcessInstance.ID.ToString();
		}

		public void Update(string ID,BPMProcessInstanceEntity BPMProcessInstance)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(BPMProcessInstance);

			this._DB.Edit(entity);
		}

        public void ProcessFinish(string ID)
        {
            var entity = this._DB.Get(new Guid(ID));
            entity.Status = 4;
            entity.FinishDate = DateTime.Now;

            this._DB.Edit(entity);
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="ID"></param>
        public bool Delete(string ID)
		{
            var guid = new Guid(ID);

            // 如果有已完的任务,禁止删除流程实例
            var taskDones = this._PMContext.BPMTaskInstanceEntity.Where(m => m.ProcessID == guid && m.Status == 2).AsEnumerable();

            if (taskDones.Count() > 0) {
                return false;
            }

            var tasks = this._PMContext.BPMTaskInstanceEntity.Where(m => m.ProcessID == guid);
            foreach (var item in tasks)
            {
                item.IsDelete = true;
                this._PMContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            var idMaps = this._PMContext.BPMProcessTaskIDMapEntity.Where(m => m.ProcessID == guid);
            foreach (var item in idMaps)
            {
                item.IsDelete = true;
                this._PMContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            var process = this._PMContext.BPMProcessInstanceEntity.SingleOrDefault(p => p.ID == guid);
            process.IsDelete = true;
            this._PMContext.Entry(process).State = System.Data.Entity.EntityState.Modified;

            this._PMContext.SaveChanges();

            return true;
		}

		public void Deletes(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(id);
            }
        }

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "BPMProcessInstance",
                ObjectID = ID,
                AttachID = AttachID
            });
        }
		
    } 
}
