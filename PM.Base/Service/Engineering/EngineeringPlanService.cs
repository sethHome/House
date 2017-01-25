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
    /// EngineeringPlan 服务
    /// </summary>
    public partial class EngineeringPlanService : IEngineeringPlanService
    {    
		private BaseRepository<EngineeringPlanEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public EngineeringPlanService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringPlanEntity>(this._PMContext);
        }

		public PageSource<EngineeringPlanInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<EngineeringPlanEntity, bool>> expression = c => true;

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
            var source = new List<EngineeringPlanInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new EngineeringPlanInfo(entity)
                {
                    
                });
            }

            return new PageSource<EngineeringPlanInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public EngineeringPlanEntity Get(int ID)
		{
			return this._DB.SingleOrDefault(p => p.EngineeringID == ID);
		}

		public int Add(EngineeringPlanInfo EngineeringPlan)
		{
			var entity = new EngineeringPlanEntity(EngineeringPlan);
            
            this._DB.Add(entity);

            //foreach (var attachID in EngineeringPlan.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
		}

		public void Update(int ID,EngineeringPlanEntity EngineeringPlan)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(EngineeringPlan);

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

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "EngineeringPlan",
                ObjectID = ID,
                AttachID = AttachID
            });
        }
		
    } 
}
