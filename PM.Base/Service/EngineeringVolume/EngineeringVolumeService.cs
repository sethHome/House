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
using PM.Base.Permission;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Chat;
using Api.Framework.Core.BaseData;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolume 服务
    /// </summary>
    public partial class EngineeringVolumeService : IEngineeringVolumeService
    {
        private BaseRepository<EngineeringVolumeEntity> _DB;
        private PMContext _PMContext;
        private string Const_BusinessKeyName = "Volume";

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IEngineeringService _IEngineeringService { get; set; }

        [Dependency("System3")]
        public IUserTaskService _IUserTaskService { get; set; }

        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public IBPMTaskInstanceService _IBPMTaskInstanceService { get; set; }

        [Dependency]
        public IEngineeringVolumeCheckService _IEngineeringVolumeCheckService { get; set; }

        [Dependency]
        public IPMPermissionCheck _IPMPermissionCheck { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public INotificationService _INotificationService { get; set; }

        [Dependency]
        public IEngineeringNoteService _IEngineeringNoteService { get; set; }

        public EngineeringVolumeService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringVolumeEntity>(this._PMContext);
        }

        public PageSource<EngineeringVolumeInfo> GetVolumePlanPagedList(PageQueryParam PageParam)
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
                    default:
                        break;
                }
            }
            #endregion

            // 判断是否可以策划全部工程的卷册，不然只能项目经理或者工程经理或者专业负责人策划卷册
            var permissionCheck = _IPMPermissionCheck.Check("DATA_VolumeAllPlan", PageParam.CurrentUser.ToString());

            var query = from en in this._PMContext.EngineeringEntity

                        join pr in this._PMContext.ProjectEntity
                        on en.ProjectID equals pr.ID

                        join es in this._PMContext.EngineeringSpecialtyEntity
                        on en.ID equals es.EngineeringID into temp1
                        from t1 in temp1.DefaultIfEmpty()

                        join vo in this._PMContext.EngineeringVolumeEntity
                        on new { engID = en.ID, specID = t1.SpecialtyID, IsDelete = false } equals new { engID = vo.EngineeringID, specID = vo.SpecialtyID, IsDelete = vo.IsDelete } into temp3
                        from t3 in temp3.DefaultIfEmpty()

                            //join p in this._PMContext.ObjectProcessEntity
                            //on new { ID = t3.ID, Key = this.Const_BusinessKeyName } equals new { ID = p.ObjectID, Key = p.ObjectKey } into temp4
                            //from t4 in temp4.DefaultIfEmpty()

                        where en.Status == (int)EnumEngineeringStatus.启动 && !en.IsDelete && t1 != null && !pr.IsDeleted &&
                            (en.Manager == PageParam.CurrentUser ||
                            pr.Manager == PageParam.CurrentUser ||
                            t1.Manager == PageParam.CurrentUser ||
                            permissionCheck != PermissionStatus.Reject)

                        //orderby en.ID descending
                        select new EngineeringProductionInfo
                        {
                            Engineering = en,
                            Project = pr,
                            Specialty = t1,
                            Volume = t3,
                            //ProcessID = t4.ProcessID
                        };

            query = query.Where(expression);

            var result = query.OrderByDescending(a => a.Engineering.ProjectID)
                .ThenByDescending(a => a.Engineering.ID)
                .ThenByDescending(a => a.Specialty.ID)
                .AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            var source = new List<EngineeringVolumeInfo>();

            foreach (var entity in result)
            {
                source.Add(new EngineeringVolumeInfo(entity)
                {
                    //Volumes = this._PMContext.EngineeringVolumeEntity.Where(v => v.EngineeringID == entity.Engineering.ID && v.SpecialtyID == entity.Specialty.SpecialtyID).ToList(),
                    //Tasks = this._IBPMTaskInstanceService.GetList(t => t.ProcessID == entity.ProcessID && t.Type == (int)TaskType.Manual && !t.IsDelete)
                });
            }

            return new PageSource<EngineeringVolumeInfo>()
            {
                Source = source,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public PageSource<EngineeringVolumeInfo> GetVolumeProcssPageList(PageQueryParam PageParam)
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
                    default:
                        break;
                }
            }
            #endregion

            // 判断是否可以策划全部工程的卷册，不然只能项目经理或者工程经理或者专业负责人策划卷册
            var p_EngProcessAll = _IPMPermissionCheck.Check("DATA_EngProcessAll", PageParam.CurrentUser.ToString());
            var p_EngProcessDept = _IPMPermissionCheck.Check("DATA_EngProcessDept", PageParam.CurrentUser.ToString());

            if (p_EngProcessAll != PermissionStatus.Reject)
            {
                // 可以查看全部进度

            }
            else if (p_EngProcessDept != PermissionStatus.Reject)
            {
                // 可以查看部门工程进度
                var users = _IDepartment.GetMyDeptUsers(PageParam.CurrentUser).AsQueryable();

                expression = expression.And(c =>
                            users.Contains(c.Engineering.Manager) ||
                            users.Contains(c.Project.Manager) ||
                            users.Contains(c.Specialty.Manager) ||
                            users.Contains(c.Volume.Designer));
            }
            else {
                // 查看与自己相关工程的进度
                expression = expression.And(c =>
                            c.Engineering.Manager == PageParam.CurrentUser ||
                            c.Project.Manager == PageParam.CurrentUser ||
                            c.Specialty.Manager == PageParam.CurrentUser ||
                            c.Volume.Designer == PageParam.CurrentUser);
            }

            var query = from en in this._PMContext.EngineeringEntity

                        join pr in this._PMContext.ProjectEntity
                        on en.ProjectID equals pr.ID

                        join es in this._PMContext.EngineeringSpecialtyEntity
                        on en.ID equals es.EngineeringID into temp1
                        from t1 in temp1.DefaultIfEmpty()

                        join vo in this._PMContext.EngineeringVolumeEntity
                        on new { engID = en.ID, specID = t1.SpecialtyID, IsDelete = false } equals new { engID = vo.EngineeringID, specID = vo.SpecialtyID, IsDelete = vo.IsDelete } into temp3
                        from t3 in temp3.DefaultIfEmpty()

                            //join p in this._PMContext.ObjectProcessEntity
                            //on new { ID = t3.ID, Key = this.Const_BusinessKeyName } equals new { ID = p.ObjectID, Key = p.ObjectKey } into temp4
                            //from t4 in temp4.DefaultIfEmpty()

                        where en.Status == (int)EnumEngineeringStatus.启动 && !en.IsDelete && t1 != null && !pr.IsDeleted

                        //orderby en.ID descending
                        select new EngineeringProductionInfo
                        {
                            Engineering = en,
                            Project = pr,
                            Specialty = t1,
                            Volume = t3,
                            //ProcessID = t4.ProcessID
                        };

            query = query.Where(expression);

            var result = query.OrderByDescending(a => a.Engineering.ProjectID)
                .ThenByDescending(a => a.Engineering.ID)
                .ThenByDescending(a => a.Specialty.ID)
                .AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            var source = new List<EngineeringVolumeInfo>();

            foreach (var entity in result)
            {
                source.Add(new EngineeringVolumeInfo(entity)
                {
                    //Volumes = this._PMContext.EngineeringVolumeEntity.Where(v => v.EngineeringID == entity.Engineering.ID && v.SpecialtyID == entity.Specialty.SpecialtyID).ToList(),
                    //Tasks = this._IBPMTaskInstanceService.GetList(t => t.ProcessID == entity.ProcessID && t.Type == (int)TaskType.Manual && !t.IsDelete)
                });
            }

            return new PageSource<EngineeringVolumeInfo>()
            {
                Source = source,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public List<EngineeringVolumeInfo> GetSpecialtyVolumes(int EngineeringID, long SpecialtyID)
        {
            var volumes = this._DB.GetList(v => v.EngineeringID == EngineeringID && v.SpecialtyID == SpecialtyID && !v.IsDelete);

            var result = new List<EngineeringVolumeInfo>();

            foreach (var v in volumes)
            {
                var processInfo = this._IObjectProcessService.Get(this.Const_BusinessKeyName, v.ID);

                result.Add(new EngineeringVolumeInfo(v)
                {
                    Tasks = this._IBPMTaskInstanceService.GetList(t => t.ProcessID == processInfo.ProcessID && t.Type == (int)TaskType.Manual && !t.IsDelete)
                });
            }

            return result;
        }

        public List<BizObject> GetSpecialtyVolumesV2(int EngineeringID, long SpecialtyID,bool WithTasks = false)
        {
            var volumes = this._DB.GetList(v => v.EngineeringID == EngineeringID && v.SpecialtyID == SpecialtyID && !v.IsDelete);
            var result = new List<BizObject>();

            foreach (var v in volumes)
            {
                var vol = new EngineeringVolumeInfo(v);

                if (WithTasks)
                {
                    var processInfo = this._IObjectProcessService.Get(this.Const_BusinessKeyName, v.ID);
                    vol.Tasks = this._IBPMTaskInstanceService.GetList(t => t.ProcessID == processInfo.ProcessID && t.Type == (int)TaskType.Manual && !t.IsDelete);
                }

                result.Add(vol);
            }

            return result;
        }

        public EngineeringVolumeEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        /// <summary>
        /// 新建卷册
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EngineeringVolume"></param>
        /// <returns></returns>
        public async Task<int> Create(int UserID,EngineeringVolumeNewInfo EngineeringVolume)
        {
            var newVol = new EngineeringVolumeEntity();

            newVol.SetEntity(EngineeringVolume);
            newVol.EngineeringID = EngineeringVolume.EngineeringID;
            newVol.SpecialtyID = EngineeringVolume.SpecialtyID;
            newVol.Designer = EngineeringVolume.TaskUsers[0].User;
            newVol.Checker = EngineeringVolume.TaskUsers[1].User;

            this._DB.Add(newVol);

            var specInfo = this._PMContext.EngineeringSpecialtyEntity.SingleOrDefault(s => s.EngineeringID == EngineeringVolume.EngineeringID && s.SpecialtyID == EngineeringVolume.SpecialtyID);

            // 新建的卷册这里开始一个生产流程
            var dicUser = new Dictionary<string, object>();
            EngineeringVolume.TaskUsers.ForEach(i =>
            {
                dicUser.Add(i.Owner, i.User);
            });

            var pid = ProcessEngine.Instance.CreateProcessInstance(specInfo.ProcessModel, UserID, dicUser);

            // 映射流程实例和卷册关系
            _IObjectProcessService.Add(new ObjectProcessEntity()
            {
                ObjectID = newVol.ID,
                ObjectKey = this.Const_BusinessKeyName,
                ProcessID = new Guid(pid)
            });

            // 设置任务的计划时间
            BPMDBService.SetTaskDate(pid, newVol.StartDate, newVol.EndDate);

            await ProcessEngine.Instance.Start(pid);

            return newVol.ID;
        }

        /// <summary>
        /// 更新卷册
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="EngineeringVolume"></param>
        public void Update(int ID, EngineeringVolumeNewInfo EngineeringVolume)
        {
            var obj = this._DB.SingleOrDefault(e => e.ID == ID);

            // 更新卷册信息
            obj.SetEntity(EngineeringVolume);
            obj.Designer = EngineeringVolume.TaskUsers[0].User;
            obj.Checker = EngineeringVolume.TaskUsers[1].User;
            this._DB.Edit(obj);

            // 更新流程用户
            var process = _IObjectProcessService.Get(this.Const_BusinessKeyName, obj.ID);

            ProcessEngine.Instance.SetProceeTaskUsers(process.ProcessID, EngineeringVolume.TaskUsers);

            // 更新任务用户
            this._IUserTaskService.ResetTaskUser(process.ProcessID, EngineeringVolume.TaskUsers);

            // 设置任务的计划时间
            BPMDBService.SetTaskDate(process.ProcessID.ToString(), obj.StartDate, obj.EndDate);
        }

        /// <summary>
        /// 删除卷册
        /// </summary>
        /// <param name="ID"></param>
        public void Delete(int ID)
        {
            // 获取卷册的流程实例ID
            var processMap = _IObjectProcessService.Get(this.Const_BusinessKeyName, ID);

            // 删除流程实例
            if (ProcessEngine.Instance.DeleteProcess(processMap.ProcessID.ToString()))
            {
                // 删除流程的任务
                _IUserTaskService.DeleteProcessTask(processMap.ProcessID);

                // 删除卷册和流程的映射关系
                _IObjectProcessService.Delete(this.Const_BusinessKeyName, ID);

                // 删除卷册
                var obj = this._DB.SingleOrDefault(e => e.ID == ID);
                obj.IsDelete = true;
                this._DB.Edit(obj);
            }
        }

        /// <summary>
        /// 批量更新卷册
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EngineeringID"></param>
        /// <param name="SpecialtyID"></param>
        /// <param name="Entitys"></param>
        /// <returns></returns>
        public async Task<List<EngineeringVolumeEntity>> BatchUpdate(int UserID, int EngineeringID, long SpecialtyID, List<EngineeringVolumeNewInfo> Entitys)
        {
            var ids = new List<int>();
            var news = new List<EngineeringVolumeEntity>();

            // 获取工程的专业信息
            var specInfo = this._PMContext.EngineeringSpecialtyEntity.SingleOrDefault(s => s.EngineeringID == EngineeringID && s.SpecialtyID == SpecialtyID);

            // 获取专业的流程信息
            //var processInfo = ProcessModelCache.Instance.GetModelInfo(specInfo.ProcessModel);

            foreach (var item in Entitys)
            {
                if (item.ID > 0)
                {
                    if (item.IsModified)
                    {
                        var obj = this._DB.SingleOrDefault(e => e.ID == item.ID);

                        // 更新卷册信息
                        obj.SetEntity(item);
                        obj.Designer = item.TaskUsers[0].User;
                        obj.Checker = item.TaskUsers[1].User;
                        this._DB.Edit(obj);

                        // 更新流程用户
                        var process = _IObjectProcessService.Get(this.Const_BusinessKeyName, obj.ID);
                        ProcessEngine.Instance.SetProceeTaskUsers(process.ProcessID, item.TaskUsers);

                        // 更新任务用户
                        this._IUserTaskService.ResetTaskUser(process.ProcessID, item.TaskUsers);

                        // 设置任务的计划时间
                        BPMDBService.SetTaskDate(process.ProcessID.ToString(), obj.StartDate, obj.EndDate);
                    }

                    ids.Add(item.ID);
                }
                else
                {
                    // 新建卷册
                    var newVol = new EngineeringVolumeEntity();

                    newVol.SetEntity(item);
                    newVol.EngineeringID = EngineeringID;
                    newVol.SpecialtyID = SpecialtyID;
                    newVol.Designer = item.TaskUsers[0].User;
                    newVol.Checker = item.TaskUsers[1].User;

                    this._DB.Add(newVol);

                    news.Add(newVol);
                    ids.Add(newVol.ID);

                    // 新建的卷册这里开始一个生产流程
                    var dicUser = new Dictionary<string, object>();
                    item.TaskUsers.ForEach(i =>
                    {
                        dicUser.Add(i.Owner, i.User);
                    });

                    var pid = ProcessEngine.Instance.CreateProcessInstance(specInfo.ProcessModel, UserID, dicUser);

                    // 映射流程实例和卷册关系
                    _IObjectProcessService.Add(new ObjectProcessEntity()
                    {
                        ObjectID = newVol.ID,
                        ObjectKey = this.Const_BusinessKeyName,
                        ProcessID = new Guid(pid)
                    });

                    // 设置任务的计划时间
                    BPMDBService.SetTaskDate(pid, newVol.StartDate, newVol.EndDate);

                    await ProcessEngine.Instance.Start(pid);
                }

            }

            var q = ids.AsQueryable();

            // 要删除的卷册
            var deleteVolumes = this._DB.GetList(v => v.EngineeringID == EngineeringID && v.SpecialtyID == SpecialtyID && !q.Contains(v.ID));

            foreach (var vol in deleteVolumes)
            {
                // 获取卷册的流程实例ID
                var processMap = _IObjectProcessService.Get(this.Const_BusinessKeyName, vol.ID);

                // 删除流程实例
                if (ProcessEngine.Instance.DeleteProcess(processMap.ProcessID.ToString()))
                {
                    // 删除流程的任务
                    _IUserTaskService.DeleteProcessTask(processMap.ProcessID);

                    // 删除卷册和流程的映射关系
                    _IObjectProcessService.Delete(this.Const_BusinessKeyName, vol.ID);

                    // 删除卷册
                    vol.IsDelete = true;
                    this._PMContext.Entry(vol).State = System.Data.Entity.EntityState.Modified;
                }
            }

            this._PMContext.SaveChanges();
            return news;
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
                ObjectKey = "EngineeringVolume",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public List<SysAttachFileEntity> GetVolumeFiles(int ID)
        {
            return _IObjectAttachService.GetAttachFiles(this.Const_BusinessKeyName, ID);
        }

        /// <summary>
        /// 获取卷册的统计信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public VolumeStatisticsInfo GetVolumeStatisticsInfo(int ID)
        {
            var data = new VolumeStatisticsInfo();

            // 1. 卷册文件数量
            data.FileCount = _IObjectAttachService.GetAttachFiles(this.Const_BusinessKeyName, ID).Count;

            // 2. 会签
            data.MuiltySignCount = 0;

            // 3.历时天数
            var objProcess = _IObjectProcessService.Get(this.Const_BusinessKeyName, ID);

            var volumeTasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == objProcess.ProcessID && !t.IsDelete && t.Type == (int)TaskType.Manual);
            var startDate = volumeTasks.First().TurnDate;
            var endDate = volumeTasks.Last().ExecuteDate;

            var diff = (endDate.HasValue ? endDate.Value : DateTime.Now) - startDate.Value;
            data.DuringDay = diff.Days;

            // 4 校审数据
            data.VolumeCheckGroups = _IEngineeringVolumeCheckService.GetVolumeCheckStatistics(ID);

            return data;
        }

        public EngineeringVolumeEntity Finish(int VolumeID)
        {
            var entity = this._DB.Get(VolumeID);

            entity.IsDone = true;
            entity.FinishDate = DateTime.Now;

            this._DB.Edit(entity);

            var vol = new EngineeringVolumeInfo(entity);

            var users = vol.GetParentMainUsers();

            var engInfo = _IEngineeringService.Get(entity.EngineeringID);

            var specInfo = _IEnum.GetEnumItemInfo("System3", "Specialty", entity.SpecialtyID.ToString());

            // 工程记事
            _IEngineeringNoteService.Add(new EngineeringNoteInfo()
            {
                Content = string.Format("卷册：[{0}]{1}完成", entity.Number, entity.Name),
                EngineeringID = entity.EngineeringID,
                NoteDate = DateTime.Now,
                NoteType = (int)EnumEngineeringNoteType.完成,
                UserID = 0,
            });

            // 给每个相关人员发送专业完成提醒
            users.ForEach(u =>
            {
                _INotificationService.Add(new NotificationInfo()
                {
                    ReceiveUser = u,
                    Head = "卷册完成",
                    Title = "卷册完成",
                    Info = string.Format("工程：{0},专业：{1}，卷册：[{2}]{3}", engInfo.Name, specInfo.Text, entity.Number,entity.Name),
                    SendUser = 0,
                    CreateDate = DateTime.Now,
                    EffectDate = DateTime.Now, // 生效日期
                    SourceID = VolumeID,
                    SourceName = "EngineeringVolume",
                    SourceTag = "Finish",
                });
            });

            return entity;
        }

        public bool IsAllVolumeDone(int EngineeringID, long SpecialtyID)
        {
            return this._DB.Count(v => v.EngineeringID == EngineeringID && v.SpecialtyID == SpecialtyID && !v.IsDelete && !v.IsDone) == 0;
        }

        /// <summary>
        /// 获取卷册信息根据任务ID
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public EngineeringVolumeInfo GetVolumeInfo(int TaskID)
        {
            var userTask = _IUserTaskService.Get(TaskID);

            var processInfo = _IObjectProcessService.Get(userTask.ProcessID.ToString());

            if (processInfo == null)
            {
                return null;
            }

            if (processInfo.ObjectKey != this.Const_BusinessKeyName)
            {
                return null;
            }


            var volume = _DB.Get(processInfo.ObjectID);

            var info = new EngineeringVolumeInfo(volume);

            info.Engineering = _PMContext.EngineeringEntity.Find(info.EngineeringID);

            return info;
        }
    }
}
