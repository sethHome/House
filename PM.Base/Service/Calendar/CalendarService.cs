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
    /// Calendar 服务
    /// </summary>
    public partial class CalendarService : ICalendarService
    {    
		private BaseRepository<CalendarEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public CalendarService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<CalendarEntity>(this._PMContext);
        }

		public PageSource<CalendarEntity> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<CalendarEntity, bool>> expression = c => !c.IsDelete;

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
                    case "ID":
                        {
                            var id = int.Parse(val);

                            if (id > 0)
                            {
                                expression = expression.And(c => c.ID == id);
                            }
                            
                            break;
                        }
                    case "User":
                        {
                            var id = int.Parse(val);
                            
                            if (id > 0)
                            {
                                expression = expression.And(c => c.UserID == id);
                            }
                            break;
                        }
                    case "Type":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.Type == id);
                            }
                            
                            break;
                        }
                    case "Today":
                        {
                            expression = expression.And(c => c.StartTime.Year == DateTime.Now.Year && c.StartTime.Month == DateTime.Now.Month && c.StartTime.Day == DateTime.Now.Day);
                            break;
                        }
                    default:
                        break;
                }
            }
			#endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
           

            return new PageSource<CalendarEntity>()
            {
                Source = pageSource.ToList(),
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public CalendarEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(CalendarInfo Calendar)
		{
			var entity = new CalendarEntity(Calendar);
            
            entity.IsDelete = false;
            entity.CreateDate = DateTime.Now;
            this._DB.Add(entity);

            //foreach (var attachID in Calendar.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
		}

		public void Update(int ID,CalendarEntity Calendar)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(Calendar);

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
                ObjectKey = "Calendar",
                ObjectID = ID,
                AttachID = AttachID
            });
        }
		
    } 
}
