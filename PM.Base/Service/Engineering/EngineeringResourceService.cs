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
    /// EngineeringResource 服务
    /// </summary>
    public partial class EngineeringResourceService : IEngineeringResourceService
    {    
		private BaseRepository<EngineeringResourceEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public EngineeringResourceService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringResourceEntity>(this._PMContext);
        }

		public PageSource<EngineeringResourceInfo> GetPagedList(PageQueryParam PageParam)
		{
            #region Filter

            Expression<Func<EngineeringResourceInfo, bool>> expression = c => true;

            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Engineering.Name.Contains(PageParam.TextCondtion) || p.Engineering.Number.Contains(PageParam.TextCondtion));
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
                            expression = expression.And(c => c.ID == id);
                            break;
                        }
                    case "Type":
                        {
                            var intVal = int.Parse(val);

                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.Type == intVal);
                            }

                            break;
                        }
                    case "Phase":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.Phase == intVal);
                            }
                            break;
                        }
                    case "VolLevel":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.VolLevel == intVal);
                            }
                            break;
                        }
                    case "TaskType":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.TaskType == intVal);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.Status == intVal);
                            }
                            break;
                        }
                    case "Manager":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Engineering.Manager == intVal);
                            }
                            break;
                        }
                    case "CreateDateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.Engineering.CreateDate >= dateVal);
                            break;
                        }
                    case "CreateDateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.Engineering.CreateDate < dateVal);
                            break;
                        }
                    case "DeliveryDateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.Engineering.DeliveryDate >= dateVal);
                            break;
                        }
                    case "DeliveryDateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.Engineering.DeliveryDate < dateVal);
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from rs in this._PMContext.EngineeringResourceEntity
                        join en in this._PMContext.EngineeringEntity
                        on rs.EngineeringID equals en.ID
                        where !en.IsDelete
                        orderby en.ID descending
                        select new EngineeringResourceInfo
                        {
                            Engineering = en,
                            ID = rs.ID,
                            Content = rs.Content,
                            EngineeringID = rs.EngineeringID,
                            CreateDate = rs.CreateDate,
                            Name = rs.Name,
                            UserID = rs.UserID
                        };

            query = query.Where(expression);

            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            return new PageSource<EngineeringResourceInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

		public EngineeringResourceEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(EngineeringResourceInfo EngineeringResource)
		{
			var entity = new EngineeringResourceEntity(EngineeringResource);
            entity.CreateDate = DateTime.Now;
            entity.IsDelete = false;
            this._DB.Add(entity);

            if (EngineeringResource.AttachIDs != null)
            {
                foreach (var attachID in EngineeringResource.AttachIDs)
                {
                    AddAttach(entity.ID, attachID);
                }
            }
            return entity.ID;
		}

		public void Update(int ID,EngineeringResourceEntity EngineeringResource)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(EngineeringResource);

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
                ObjectKey = "EngineeringResource",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public List<BizObject> GetEngineeringResource(int EngineeringID)
        {
            var list = _DB.GetList(e => !e.IsDelete && e.EngineeringID == EngineeringID);
            var result = new List<BizObject>();

            foreach (var item in list)
            {
                result.Add(new EngineeringResourceInfo(item));
            }
            return result;
        }
		
    } 
}
