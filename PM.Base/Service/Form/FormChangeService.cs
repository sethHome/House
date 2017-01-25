using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using BPM.Engine;
using System.Threading.Tasks;
using System.Web.Http;
using BPM.DB;

namespace PM.Base
{   
    /// <summary>
    /// FormChange 服务
    /// </summary>
    public partial class FormChangeService : IFormChangeService
    {    
		private BaseRepository<FormChangeEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }
        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }
        
        public FormChangeService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<FormChangeEntity>(this._PMContext);
        }

		public PageSource<FormChangeInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<FormChangeInfo, bool>> expression = c => true;

            #region Filter
            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                var engIDs = (from eng in
                            this._PMContext.Set<EngineeringEntity>().Where(
                                     o => o.Name.Contains(PageParam.TextCondtion)
                                       || o.Number.Contains(PageParam.TextCondtion))
                              select eng.ID);

                expression = expression.And(p => engIDs.Contains(p.Engineering.ID));
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
                    case "Specialty":
                        {
                            var intVal = long.Parse(val);
                            if (intVal > 0L)
                            {
                                expression = expression.And(c => c.SpecialtyID == intVal);
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
                    case "Self":
                        {
                            if (val.ToLower() == "true")
                            {
                                expression = expression.And(c => c.CreateUserID == PageParam.CurrentUser);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from fc in this._PMContext.FormChangeEntity

                        join en in this._PMContext.EngineeringEntity
                        on fc.EngineeringID equals en.ID

                        join ev in this._PMContext.EngineeringVolumeEntity
                        on fc.VolumeID equals ev.ID

                        where !en.IsDelete && !fc.IsDelete

                        select new FormChangeInfo
                        {
                            ID = fc.ID,
                            EngineeringID = fc.EngineeringID,
                            SpecialtyID = fc.SpecialtyID,
                            VolumeID = fc.VolumeID,
                            CopyCustomerID = fc.CopyCustomerID,
                            MainCustomerID = fc.MainCustomerID,
                            Reason = fc.Reason,
                            Content = fc.Content,
                            AttachID = fc.AttachID,
                            CreateDate = fc.CreateDate,
                            CreateUserID = fc.CreateUserID,
                            
                            Engineering = en,
                            Volume = ev,
                        };

            var result = query.Where(expression)
                        .MyOrder<FormChangeInfo>(PageParam.OrderFiled, PageParam.IsDesc)
                        .ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            return new PageSource<FormChangeInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

		public FormChangeInfo Get(int ID)
		{
            var changeInfo = new FormChangeInfo(this._DB.Get(ID));

            changeInfo.Engineering = _PMContext.EngineeringEntity.Find(changeInfo.EngineeringID);
            changeInfo.Volume = _PMContext.EngineeringVolumeEntity.Find(changeInfo.VolumeID);
            changeInfo.MainCustomer = _PMContext.CustomerEntity.Find(changeInfo.MainCustomerID);
            changeInfo.CopyCustomer = _PMContext.CustomerEntity.Find(changeInfo.CopyCustomerID);

            return changeInfo;
		}

		public async Task<int> Add(FormChangeInfo FormChange)
		{
			var entity = new FormChangeEntity(FormChange);
            entity.CreateDate = DateTime.Now;
            entity.IsDelete = false;
            this._DB.Add(entity);

            foreach (var attachID in FormChange.AttachIDs)
            {
                AddAttach(entity.ID, attachID);
            }

            var pid = ProcessEngine.Instance.CreateProcessInstance("Form_Change", FormChange.CreateUserID, FormChange.FlowData);

            // 映射流程实例和卷册关系
            _IObjectProcessService.Add(new ObjectProcessEntity()
            {
                ObjectID = entity.ID,
                ObjectKey = "FormChange",
                ProcessID = new Guid(pid)
            });

            await ProcessEngine.Instance.Start(pid);

            return entity.ID;
		}

		public void Update(int ID,FormChangeEntity FormChange)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(FormChange);

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

        public IHttpActionResult Export(int ID)
        {
            var changeInfo = this.Get(ID);

            return new ExportFormWord("R737-02设计变更单.doc", "变更单.doc");
        }

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "FormChange",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public List<BizObject> GetVolumeChanges(int VolumeID)
        {
            var list = this._DB.GetList(c => c.VolumeID == VolumeID);
            var result = new List<BizObject>();
            foreach (var item in list)
            {
                result.Add(new FormChangeInfo(item));
            }

            return result;
        }
    } 
}
