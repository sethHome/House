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
    /// EngineeringNote 服务
    /// </summary>
    public partial class EngineeringNoteService : IEngineeringNoteService
    {    
		private BaseRepository<EngineeringNoteEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }


        [Dependency]
        public INotificationService _INotificationService { get; set; }

        public EngineeringNoteService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringNoteEntity>(this._PMContext);
        }

		public PageSource<EngineeringNoteInfo> GetPagedList(PageQueryParam PageParam)
		{
            #region Filter

            Expression<Func<EngineeringNoteInfo, bool>> expression = c => true;

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
                    case "IDs":
                        {
                            var ids = val.Split(',').Select(i => int.Parse(i)).AsQueryable();
                            expression = expression.And(c => ids.Contains(c.Engineering.ID));
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

            var query = from nt in this._PMContext.EngineeringNoteEntity
                        join en in this._PMContext.EngineeringEntity
                        on nt.EngineeringID equals en.ID
                        where !en.IsDelete
                        orderby en.ID descending
                        select new EngineeringNoteInfo
                        {
                            Engineering = en,
                            ID = nt.ID,
                            Content = nt.Content,
                            EngineeringID = nt.EngineeringID,
                            NoteType = nt.NoteType,
                            NoteDate = nt.NoteDate,
                            UserID = nt.UserID
                        };

            query = query.Where(expression);

            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            return new PageSource<EngineeringNoteInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

		public EngineeringNoteInfo Get(int ID)
		{
			var entity = this._DB.Get(ID);

            var info = new EngineeringNoteInfo(entity);
            info.Engineering = this._PMContext.EngineeringEntity.Find(entity.EngineeringID);

            return info;
		}

		public int Add(EngineeringNoteInfo EngineeringNote)
		{
			var entity = new EngineeringNoteEntity(EngineeringNote);

            entity.IsDeleted = false;
            entity.NoteDate = DateTime.Now;
            this._DB.Add(entity);

            if (EngineeringNote.AttachIDs != null) {
                foreach (var attachID in EngineeringNote.AttachIDs)
                {
                    AddAttach(entity.ID, attachID);
                }
            }

            // 给指定接收人发送提醒
            if (EngineeringNote.ReceiveUsers != null) {
                foreach (var userID in EngineeringNote.ReceiveUsers)
                {
                    _INotificationService.Add(new NotificationInfo()
                    {
                        CreateDate = DateTime.Now,
                        EffectDate = DateTime.Now, // 生效日期
                        Title = string.Format("工程记事:{0}", (EnumEngineeringNoteType)EngineeringNote.NoteType),
                        Info = EngineeringNote.Content,
                        ReceiveUser = userID,
                        SendUser = EngineeringNote.UserID,
                        SourceID = entity.ID,
                        SourceName = "EngineeringNote",
                        SourceTag = "Note",
                    });

                }
            }

            return entity.ID;
		}

		public void Update(int ID,EngineeringNoteEntity EngineeringNote)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(EngineeringNote);

			this._DB.Edit(entity);
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

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "EngineeringNote",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public List<EngineeringNoteEntity> GetEngineeringNote(int EngineeringID)
        {
            return _DB.GetList(nt => !nt.IsDeleted && nt.EngineeringID == EngineeringID).OrderByDescending(nt => nt.NoteDate).ToList();
        }
    } 
}
