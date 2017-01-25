using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

using Api.Framework.Core.BaseData;
using Api.Framework.Core.Permission;
using PM.Base.Permission;

namespace PM.Base
{
    /// <summary>
    /// EngineeringSpecialty 服务
    /// </summary>
    public partial class EngineeringSpecialtyService : IEngineeringSpecialtyService
    {
        private BaseRepository<EngineeringSpecialtyEntity> _DB;
        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IPMPermissionCheck _IPMPermissionCheck { get; set; }

        [Dependency]
        public IEngineeringVolumeService _IEngineeringVolumeService { get; set; }

        [Dependency]
        public IEngineeringService _IEngineeringService { get; set; }

        [Dependency]
        public INotificationService _INotificationService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        [Dependency]
        public IEngineeringNoteService _IEngineeringNoteService { get; set; }

        public EngineeringSpecialtyService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringSpecialtyEntity>(this._PMContext);
        }

        public PageSource<EngineeringSpecialtyInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<EngineeringProductionInfo, bool>> expression = c => true;

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
                            var longVal = long.Parse(val);
                            if (longVal > 0L)
                            {
                                expression = expression.And(c => c.Specialty.SpecialtyID == longVal);
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

            // 判断是否可以策划全部工程的专业，不然只能策划项目经理或者工程经理是自己的工程
            var permissionCheck = _IPMPermissionCheck.Check("DATA_SpecialtyAllPlan", PageParam.CurrentUser.ToString());

            var query = from en in this._PMContext.EngineeringEntity
                        join es in this._PMContext.EngineeringSpecialtyEntity
                        on en.ID equals es.EngineeringID into temp
                        join pr in this._PMContext.ProjectEntity
                        on en.ProjectID equals pr.ID
                        from tt in temp.DefaultIfEmpty()
                        where en.Status == (int)EnumEngineeringStatus.启动 && !en.IsDelete && !pr.IsDeleted
                            && (en.Manager == PageParam.CurrentUser || pr.Manager == PageParam.CurrentUser || permissionCheck != PermissionStatus.Reject)

                        select new EngineeringProductionInfo
                        {
                            Engineering = en,
                            Project = pr,
                            Specialty = tt
                        };

            query = query.Where(expression);

            var result = query.OrderByDescending(a => a.Engineering.ProjectID).ThenByDescending(a => a.Engineering.ID).AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            var source = new List<EngineeringSpecialtyInfo>();

            foreach (var entity in result)
            {
                source.Add(new EngineeringSpecialtyInfo(entity)
                {
                    Engineering = entity.Engineering,
                    Project = entity.Project,
                    Specialtys = entity.Specialty == null ? null :
                        this._PMContext.EngineeringSpecialtyEntity
                        .Where(e => e.EngineeringID == entity.Engineering.ID).ToList()
                });
            }

            return new PageSource<EngineeringSpecialtyInfo>()
            {
                Source = source,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public EngineeringSpecialtyEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public int Add(EngineeringSpecialtyInfo EngineeringSpecialty)
        {
            var entity = new EngineeringSpecialtyEntity(EngineeringSpecialty);

            this._DB.Add(entity);

            //foreach (var attachID in EngineeringSpecialty.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
        }

        public void Update(int ID, List<EngineeringSpecialtyEntity> Entitys)
        {
            //var specils = _DB.GetList(e => e.EngineeringID == ID);
            //var ids = new List<long>();
            var ids = 0L;
            foreach (var item in Entitys)
            {
                var obj = this._DB.SingleOrDefault(e => e.EngineeringID == ID && e.SpecialtyID == item.SpecialtyID);

                if (obj == null)
                {
                    // add
                    item.EngineeringID = ID;
                    this._DB.Add(item);
                }
                else
                {
                    // update
                    obj.SetEntity(item);
                    this._DB.Edit(obj);
                }

                ids = (ids | item.SpecialtyID);
            }

            // 删除专业下的卷册
            var specils = this._DB.GetList(e => e.EngineeringID == ID && (ids & e.SpecialtyID) == 0);

            foreach (var s in specils)
            {
                var volumes = _IEngineeringVolumeService.GetSpecialtyVolumesV2(ID, s.SpecialtyID);

                foreach (var item in volumes)
                {
                    _IEngineeringVolumeService.Delete(item.ID);
                }
            }

            // 删除专业
            this._DB.Delete(e => e.EngineeringID == ID && (ids & e.SpecialtyID) == 0);
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
                ObjectKey = "EngineeringSpecialty",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public List<EngineeringVolumeFileInfo> GetSpecialtyAttachs(int EngineeringID, long SpecialtyID, int UserID = 0)
        {
            var volumes = from v in this._PMContext.EngineeringVolumeEntity
                          where !v.IsDelete && v.EngineeringID == EngineeringID && (v.SpecialtyID & SpecialtyID) > 0
                          select v;

            var result = new List<EngineeringVolumeFileInfo>();

            foreach (var vol in volumes)
            {
                var files = _IObjectAttachService.GetAttachFiles("Volume", vol.ID, UserID);
                if (files.Count > 0)
                {
                    result.Add(new EngineeringVolumeFileInfo()
                    {
                        Volume = vol,
                        Files = files
                    });
                }
            }
            return result;
        }

        public List<EngineeringVolumeEntity> GetSpecialtyVolumes(int EngineeringID, long SpecialtyID)
        {
            var volumes = from v in this._PMContext.EngineeringVolumeEntity
                          where !v.IsDelete && v.EngineeringID == EngineeringID && (v.SpecialtyID & SpecialtyID) > 0
                          select v;


            return volumes.ToList();
        }

        public List<EngineeringSpecialtyEntity> GetEngineeringSpecialtyValue(int EngineeringID)
        {
            //var query = from s in _PMContext.EngineeringSpecialtyEntity
            //            where s.EngineeringID == EngineeringID
            //            select s.SpecialtyID;

            //return query.ToList();

            return _DB.GetList(s => s.EngineeringID == EngineeringID).ToList();
        }

        public List<BizObject> GetEngineeringSpecialtys(int EngineeringID)
        {
            var query = _DB.GetList(s => s.EngineeringID == EngineeringID);

            var result = new List<BizObject>();

            var dicSpecEnums = _IEnum.GetEnumDic("System3", "Specialty");

            foreach (var item in query)
            {
                result.Add(new EngineeringSpecialtyInfo(item)
                {
                    ObjectText = dicSpecEnums[item.SpecialtyID.ToString()]
                });
            }
            return result;
        }

        public EngineeringSpecialtyInfo Get(int EngineeringID, long SpecialtyID)
        {
            var entity = _DB.SingleOrDefault(s => s.EngineeringID == EngineeringID && s.SpecialtyID == SpecialtyID);
            var dicSpecEnums = _IEnum.GetEnumDic("System3", "Specialty");
            return new EngineeringSpecialtyInfo(entity)
            {
                ObjectText = dicSpecEnums[SpecialtyID.ToString()]
            };
        }

        /// <summary>
        /// 判断工程下的专业是否全部完成
        /// </summary>
        /// <param name="EngineeringID"></param>
        /// <returns></returns>
        public bool IsAllSpecialtyDone(int EngineeringID)
        {
            return this._DB.Count(s => s.EngineeringID == EngineeringID && !s.IsDone) == 0;
        }

        /// <summary>
        /// 专业完成
        /// </summary>
        /// <param name="EngineeringID"></param>
        /// <param name="SpecialtyID"></param>
        public void Finish(int EngineeringID, long SpecialtyID)
        {
            var entity = this._DB.SingleOrDefault(s => s.EngineeringID == EngineeringID && s.SpecialtyID == SpecialtyID);

            entity.IsDone = true;
            entity.FinishDate = DateTime.Now;

            this._DB.Edit(entity);

            var specInfo = new EngineeringSpecialtyInfo(entity);

            var eng = specInfo.GetParent();

            var users = specInfo.GetParentMainUsers();

            if (!users.Contains(entity.Manager))
            {
                users.Add(entity.Manager);
            }

            var specEnumInfo = _IEnum.GetEnumItemInfo("System3", "Specialty", SpecialtyID.ToString());

            // 工程记事
            _IEngineeringNoteService.Add(new EngineeringNoteInfo()
            {
                Content = string.Format("{0}完成", specEnumInfo.Text),
                EngineeringID = entity.EngineeringID,
                NoteDate = DateTime.Now,
                NoteType = (int)EnumEngineeringNoteType.完成,
                UserID = 0
            });

            // 给每个相关人员发送专业完成提醒
            users.ForEach(u =>
            {
                _INotificationService.Add(new NotificationInfo()
                {
                    ReceiveUser = u,
                    Head = "专业完成",
                    Title = "专业完成",
                    Info = string.Format("工程:{0},专业:{1}", eng.ObjectText, specEnumInfo.Text),
                    SendUser = 0,
                    CreateDate = DateTime.Now,
                    EffectDate = DateTime.Now, // 生效日期
                    SourceID = specInfo.ID,
                    SourceName = "EngineeringSpecialty",
                    SourceTag = "Finish",
                });
            });
        }
    }
}
