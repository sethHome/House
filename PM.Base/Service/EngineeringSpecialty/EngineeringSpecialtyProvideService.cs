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
using BPM.DB;

namespace PM.Base
{
    /// <summary>
    /// EngineeringSpecialtyProvide 服务
    /// </summary>
    public partial class EngineeringSpecialtyProvideService : IEngineeringSpecialtyProvideService
    {
        private BaseRepository<EngineeringSpecialtyProvideEntity> _DB;
        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }
        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        public EngineeringSpecialtyProvideService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringSpecialtyProvideEntity>(this._PMContext);
        }

        public PageSource<EngineeringSpecialtyProvideInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<EngineeringSpecialtyProvideInfo, bool>> expression = c => true;

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
                                expression = expression.And(c => c.Specialty.SpecialtyID == intVal);
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
                    case "MyReceive":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                var id = "," + PageParam.CurrentUser.ToString() + ",";

                                expression = expression.And(c => c.CanReceive && (!c.LimitDate.HasValue || c.LimitDate >= DateTime.Now) && ("," + c.ReceiveUserIDs + ",").IndexOf(id) >= 0);
                            }
                            else
                            {
                                var volQuery = from v in this._PMContext.EngineeringVolumeEntity
                                               where v.Designer == PageParam.CurrentUser && !v.IsDelete
                                               select new
                                               {
                                                   v.EngineeringID,
                                                   v.SpecialtyID
                                               };

                                expression = expression.And(c => volQuery.Contains(new { c.EngineeringID, c.Specialty.SpecialtyID }));
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            //var volQuery = from v in this._PMContext.EngineeringVolumeEntity
            //               where v.Designer == PageParam.CurrentUser && !v.IsDelete
            //               select new
            //               {
            //                   v.EngineeringID,
            //                   v.SpecialtyID
            //               };

            var query = from en in this._PMContext.EngineeringEntity

                        join es in this._PMContext.EngineeringSpecialtyEntity
                        on en.ID equals es.EngineeringID into temp1
                        from t1 in temp1.DefaultIfEmpty()

                        join sp in this._PMContext.EngineeringSpecialtyProvideEntity
                        on new { engID = en.ID, specID = t1.SpecialtyID, IsDelete = false } equals
                            new { engID = sp.EngineeringID, specID = sp.SendSpecialtyID, IsDelete = sp.IsDelete } into temp2
                        from t2 in temp2.DefaultIfEmpty()

                        where (en.Status == (int)EnumEngineeringStatus.启动 || en.Status == (int)EnumEngineeringStatus.完成)
                        && !en.IsDelete && t1 != null
                        //&& volQuery.Contains(new { t1.EngineeringID ,t1.SpecialtyID })

                        select new EngineeringSpecialtyProvideInfo
                        {
                            ID = t2 == null ? 0 : t2.ID,
                            DocName = t2 == null ? "" : t2.DocName,
                            CreateDate = t2 == null ? DateTime.Now : t2.CreateDate,
                            LimitDate = t2 == null ? null : t2.LimitDate,
                            DocContent = t2 == null ? "" : t2.DocContent,
                            EngineeringID = en.ID,
                            ReceiveSpecialtyID = t2 == null ? 0l : t2.ReceiveSpecialtyID,
                            ReceiveUserIDs = t2 == null ? "" : t2.ReceiveUserIDs,
                            SendSpecialtyID = t1.SpecialtyID,
                            SendUserID = t2 == null ? 0 : t2.SendUserID,
                            CanReceive = t2 == null ? false : t2.CanReceive,
                            Engineering = en,
                            Specialty = t1
                        };

            query = query.Where(expression).MyOrder<EngineeringSpecialtyProvideInfo>(PageParam.OrderFiled, PageParam.IsDesc); ;

            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            return new PageSource<EngineeringSpecialtyProvideInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public EngineeringSpecialtyProvideEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public async Task<int> Add(EngineeringSpecialtyProvideInfo EngineeringSpecialtyProvide)
        {
            var entity = new EngineeringSpecialtyProvideEntity(EngineeringSpecialtyProvide);
            entity.IsDelete = false;
            entity.CreateDate = DateTime.Now;
            entity.CanReceive = false;

            this._DB.Add(entity);

            foreach (var attachID in EngineeringSpecialtyProvide.AttachIDs)
            {
                AddAttach(entity.ID, attachID);
            }
            foreach (var fileID in EngineeringSpecialtyProvide.VolumeFiles)
            {
                AddAttach(entity.ID, fileID);
            }

            var data = new Dictionary<string, object>();
            data.Add("ApproveUser", EngineeringSpecialtyProvide.ApproveUser);
            data.Add("ProvideUser", EngineeringSpecialtyProvide.SendUserID);

            var pid = ProcessEngine.Instance.CreateProcessInstance("F1", EngineeringSpecialtyProvide.SendUserID, data);

            // 映射流程实例和卷册关系
            _IObjectProcessService.Add(new ObjectProcessEntity()
            {
                ObjectID = entity.ID,
                ObjectKey = "EngineeringSpecialtyProvide",
                ProcessID = new Guid(pid)
            });

            await ProcessEngine.Instance.Start(pid);

            return entity.ID;
        }

        public void Update(int ID, EngineeringSpecialtyProvideInfo Info)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(Info);

            this._DB.Edit(entity);

            foreach (var fileID in Info.VolumeFiles)
            {
                AddAttach(entity.ID, fileID);
            }
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
                ObjectKey = "EngineeringSpecialtyProvide",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public void CanReceive(int ID)
        {
            var entity = this._DB.Get(ID);

            entity.CanReceive = true;

            this._DB.Edit(entity);
        }
    }
}
