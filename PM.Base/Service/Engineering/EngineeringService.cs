using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using System.Linq;
using Api.Framework.Core.BaseData;
using PM.Base.Permission;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Chat;
using Api.Framework.Core.User;
using BPM.DB;

namespace PM.Base
{
    /// <summary>
    /// Engineering 服务
    /// </summary>
    public partial class EngineeringService : IEngineeringService, ITopBizObject
    {
        private BaseRepository<EngineeringEntity> _DB;

        private PMContext _PMContext;

        private List<int> _DeptUsers;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IEnum _IEnum { get; set; }

        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public IPMPermissionCheck _IPMPermissionCheck { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public INotificationService _INotificationService { get; set; }

        [Dependency]
        public IEngineeringNoteService _IEngineeringNoteService { get; set; }

        [Dependency]
        public IUserConfig _IUserConfig { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        public EngineeringService()
        {
            _PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringEntity>(_PMContext);
        }

        public PageSource<EngineeringInfo> GetPagedList(PageQueryParam PageParam)
        {
            #region Filter

            Expression<Func<EngineeringEntity, bool>> expression = c => c.IsDelete == PageParam.IsDelete;

            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                if (filter.Value == null)
                {
                    continue;
                }
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
                            expression = expression.And(c => ids.Contains(c.ID));
                            break;
                        }
                    case "Type":
                        {
                            var intVal = int.Parse(val);

                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Type == intVal);
                            }

                            break;
                        }
                    case "Phase":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Phase == intVal);
                            }
                            break;
                        }
                    case "VolLevel":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.VolLevel == intVal);
                            }
                            break;
                        }
                    case "TaskType":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.TaskType == intVal);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Status == intVal);
                            }
                            break;
                        }
                    case "Manager":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Manager == intVal);
                            }
                            break;
                        }
                    case "CreateDateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.CreateDate >= dateVal);
                            break;
                        }
                    case "CreateDateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.CreateDate < dateVal);
                            break;
                        }
                    case "DeliveryDateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.DeliveryDate >= dateVal);
                            break;
                        }
                    case "DeliveryDateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.DeliveryDate < dateVal);
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            #region 权限

            var p_All = _IPMPermissionCheck.Check("DATA_Engineering_All", PageParam.CurrentUser.ToString());

            if (p_All == PermissionStatus.Reject)
            {
                var p_Dept = _IPMPermissionCheck.Check("DATA_Engineering_Dept", PageParam.CurrentUser.ToString());
                if (p_Dept != PermissionStatus.Reject)
                {
                    // 可以查看部门的工程
                    var _DeptUsers = _IDepartment.GetMyDeptUsers(PageParam.CurrentUser).AsQueryable();
                    expression = expression.And(e => _DeptUsers.Contains(e.Manager));
                }
                else
                {
                    // 查看与自己相关的工程
                    expression = expression.And(e => e.Manager == PageParam.CurrentUser);
                }
            }
            #endregion

            PageParam.OrderFiled = "ProjectID";
            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<EngineeringInfo>();

            foreach (var entity in pageSource)
            {
                source.Add(new EngineeringInfo(entity)
                {
                    ProjectInfo = _PMContext.ProjectEntity.Find(entity.ProjectID)
                });
            }

            return new PageSource<EngineeringInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public List<EngineeringEntity> GetSource(int count, Dictionary<string, string> Conditions)
        {
            //return _DB.GetList(e => e.Name.Contains(TextFilter) || e.Number.Contains(TextFilter)).ToPagedList();

            Expression<Func<EngineeringEntity, bool>> expression = c => !c.IsDelete;

            if (!string.IsNullOrEmpty(Conditions["TxtFilter"]))
            {
                var filter = Conditions["TxtFilter"];
                expression = expression.And(p => p.Number.Contains(filter) || p.Name.Contains(filter));
            }
            if (!string.IsNullOrEmpty(Conditions["ExceptIds"]))
            {
                var ids = Conditions["ExceptIds"].Split(',');
                foreach (var id in ids)
                {
                    var intVal = int.Parse(id);
                    expression = expression.And(p => p.ID != intVal);
                }
            }
            if (!string.IsNullOrEmpty(Conditions["Name"]))
            {
                var val = Conditions["Name"];
                expression = expression.And(p => p.Name.Contains(val));
            }
            if (!string.IsNullOrEmpty(Conditions["Number"]))
            {
                var val = Conditions["Number"];
                expression = expression.And(p => p.Number.Contains(val));
            }

            return _DB.GetList(expression).Take(count).ToList();
        }

        public EngineeringInfo Get(int ID)
        {
            var entity = this._DB.Get(ID);
            var proj = this._PMContext.ProjectEntity.Find(entity.ProjectID);

            var result = new EngineeringInfo(entity);
            result.ProjectInfo = proj;

            return result;
        }

        public int Add(EngineeringInfo Engineering)
        {
            var entity = new EngineeringEntity(Engineering);
            entity.IsDelete = false;
            entity.Status = (int)EnumEngineeringStatus.新建;
            this._DB.Add(entity);

            //  启动工程
            Start(entity.ID, Engineering.CreateUser);

            return entity.ID;
        }

        public void Update(int ID, EngineeringEntity Engineering)
        {
            var entity = this._DB.Get(ID);
            entity.SetEntity(Engineering);

            this._DB.Edit(entity);
        }

        public void BackUp(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                var entity = this._DB.Get(int.Parse(id));
                entity.IsDelete = false;
                this._DB.Edit(entity);
            }
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDelete = true;
            this._DB.Edit(entity);
        }

        private void AddAttach(int ProjID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "Engineering",
                ObjectID = ProjID,
                AttachID = AttachID
            });
        }

        public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

        /// <summary>
        /// 工程甘特图数据
        /// </summary>
        /// <param name="PageParam"></param>
        /// <returns></returns>
        public List<EngineeringGanttInfo> GetEngineeringGantt(int ID, int CurrentUser)
        {
            var param = new PageQueryParam()
            {
                CurrentUser = CurrentUser
            };

            param.FilterCondtion = new Hashtable();
            param.FilterCondtion.Add("Engineering.ID", ID);

            var treeObjs = this.Get(param);

            var source = new List<EngineeringGanttInfo>();
            var ganntObjectKeys = new List<string>() { "Engineering", "Specialty", "Volume" };

            var removeItems = new List<EngineeringGanttInfo>();

            var allGannt = fill(removeItems, treeObjs, "", ganntObjectKeys);

            removeNoTaskItems(allGannt, removeItems);

            return allGannt;

        }

        private List<EngineeringGanttInfo> fill(List<EngineeringGanttInfo> removeItems, List<BizObject> objTrees, string parentID, List<string> ganntObjectKeys)
        {
            var ganntItems = new List<EngineeringGanttInfo>();

            foreach (var obj in objTrees)
            {
                if (!ganntObjectKeys.Contains(obj.ObjectKey))
                {
                    continue;
                }

                var gannt = new EngineeringGanttInfo()
                {
                    ID = obj.ObjectID,
                    Name = obj.ObjectText,
                    Parent = parentID,
                };

                if (obj.HasTask)
                {
                    var processID = _IObjectProcessService.Get(obj.ObjectKey, obj.ID).ProcessID;

                    var task = BPM.Engine.BPMDBService.GetProcessTasks(processID);

                    gannt.Tasks = new List<ProcessNodeInfo>();
                    foreach (var t in task)
                    {
                        gannt.Tasks.Add(new ProcessNodeInfo()
                        {
                            ID = t.ID.ToString(),
                            Name = t.Name,
                            From = t.TurnDate.HasValue && t.SourceID != "_3" ? t.TurnDate : t.EstDate,
                            To = t.ExecuteDate.HasValue ? t.ExecuteDate : t.LctDate,
                            Est = t.EstDate,
                            Lct = t.LctDate,
                            State = t.Status,
                            User = t.UserID
                        });
                    }
                }

                if (obj.Children != null && obj.Children.Count > 0)
                {
                    var childGanntItems = fill(removeItems, obj.Children, obj.ObjectID, ganntObjectKeys);

                    ganntItems.AddRange(childGanntItems);
                }
                else if (!obj.HasTask)
                {
                    removeItems.Add(gannt);
                }

                ganntItems.Add(gannt);
            }

            return ganntItems;
        }

        private void removeNoTaskItems(List<EngineeringGanttInfo> items, List<EngineeringGanttInfo> removeItem)
        {
            foreach (var item in removeItem)
            {
                if (items.Count(g => g.Parent == item.ID) == 0)
                {
                    items.Remove(item);
                }

                var query = items.Where(g => g.ID == item.Parent).ToList();

                removeNoTaskItems(items, query);
            }
        }

        public List<BizObject> GetListByProjectID(int ProjectID, PageQueryParam PageParam)
        {
            #region Filter

            Expression<Func<EngineeringEntity, bool>> expression = c => !c.IsDelete && c.ProjectID == ProjectID;
            if (PageParam.TextCondtion != null && !string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var value = filter.Value.ToString();

                if (value == "0" || string.IsNullOrEmpty(value))
                {
                    continue;
                }


                switch (filter.Key.ToString())
                {
                    case "Engineering.Type":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Type == intVal);

                            break;
                        }
                    case "Engineering.Phase":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Phase == intVal);
                            break;
                        }
                    case "Engineering.VolLevel":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.VolLevel == intVal);
                            break;
                        }
                    case "Engineering.TaskType":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.TaskType == intVal);
                            break;
                        }
                    case "Engineering.Status":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Status == intVal);
                            break;
                        }
                    case "Engineering.Manager":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Manager == intVal);
                            break;
                        }
                    case "Engineering.DateRange":
                        {
                            var intVal = int.Parse(value);
                            switch (intVal)
                            {
                                case 0: break;
                                case 1: break;
                                case 2: break;
                                case 3: break;
                                case 4: break;
                                default:
                                    break;
                            }
                            break;
                        }

                    default:
                        break;
                }
            }
            #endregion

            var list = _DB.GetList(expression);
            var result = new List<BizObject>();

            foreach (var item in list)
            {
                result.Add(new EngineeringInfo(item));
            }
            return result;
        }

        /// <summary>
        /// 工程暂停
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Reason"></param>
        public void Stop(int ID, int UserID, string Reason, List<int> ReceiveUserIDs = null)
        {
            var entity = this._DB.Get(ID);
            entity.Status = (int)EnumEngineeringStatus.暂停;
            entity.StopDate = DateTime.Now;
            this._DB.Edit(entity);

            var engInfo = new EngineeringInfo(entity);

            // 项目负责人
            var users = engInfo.GetParentMainUsers();

            // 工程相关人员
            users.AddRange(engInfo.GetChildrenMainUsers());

            // 其他相关人员
            if (ReceiveUserIDs != null)
            {
                users.AddRange(ReceiveUserIDs);
            }

            // 去除重复人员
            var receiveUserIDs = users.Where((x, i) => users.FindIndex(z => z == x) == i).ToList();

            _IEngineeringNoteService.Add(new EngineeringNoteInfo()
            {
                Content = string.Format("工程：{0}暂停，暂停时间：{1},说明：{2}", entity.Name, entity.StopDate, Reason),
                EngineeringID = ID,
                NoteDate = DateTime.Now,
                NoteType = (int)EnumEngineeringNoteType.暂停,
                UserID = UserID,
                ReceiveUsers = receiveUserIDs,
                Engineering = entity,
            });
        }

        /// <summary>
        /// 工程启动
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Reason"></param>
        public void Start(int ID, int UserID, List<int> ReceiveUserIDs = null)
        {
            var entity = this._DB.Get(ID);
            entity.Status = (int)EnumEngineeringStatus.启动;
            entity.StartDate = DateTime.Now;
            this._DB.Edit(entity);

            var engInfo = new EngineeringInfo(entity);

            // 项目负责人
            var users = engInfo.GetParentMainUsers();

            // 工程负责人
            users.Add(entity.Manager);

            // 其他相关人员
            if (ReceiveUserIDs != null)
            {
                users.AddRange(ReceiveUserIDs);
            }

            // 去除重复人员
            var receiveUserIDs = users.Where((x, i) => users.FindIndex(z => z == x) == i).ToList();

            _IEngineeringNoteService.Add(new EngineeringNoteInfo()
            {
                Content = string.Format("工程{0}启动", entity.Name),
                EngineeringID = ID,
                NoteDate = DateTime.Now,
                NoteType = (int)EnumEngineeringNoteType.启动,
                UserID = UserID,
                ReceiveUsers = receiveUserIDs,
                Engineering = entity,
            });
        }

        /// <summary>
        /// 工程完成
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Reason"></param>
        public void Finish(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.Status = (int)EnumEngineeringStatus.完成;
            entity.FinishDate = DateTime.Now;
            this._DB.Edit(entity);

            var engInfo = new EngineeringInfo(entity);

            // 项目负责人
            var users = engInfo.GetParentMainUsers();

            // 工程负责人
            users.Add(entity.Manager);

            // 去除重复人员
            users = users.Where((x, i) => users.FindIndex(z => z == x) == i).ToList();

            _IEngineeringNoteService.Add(new EngineeringNoteInfo()
            {
                Content = "工程完成",
                EngineeringID = ID,
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
                    Head = "工程完成",
                    Title = "工程完成",
                    Info = entity.Name,
                    SendUser = 0,
                    CreateDate = DateTime.Now,
                    EffectDate = DateTime.Now, // 生效日期
                    SourceID = ID,
                    SourceName = "Engineering",
                    SourceTag = "Finish",
                });
            });
        }

        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserID"></param>
        public int Follow(int ID, int UserID)
        {
            var userFollows = _IUserConfig.GetUserConfig(UserID, "EngineeringFollow");

            if (userFollows.Count(f => f.ConfigValue == ID.ToString()) > 0)
            {
                return 1;
            }

            _IUserConfig.AddConfig(new UserConfigEntity()
            {
                ConfigName = "EngineeringFollow",
                ConfigKey = ID.ToString(),
                UserID = UserID
            });

            return 0;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserID"></param>
        public void UnFollow(int ID, int UserID)
        {
            _IUserConfig.RemoveConfig(UserID, "EngineeringFollow", ID.ToString());
        }

        public bool IsFollow(int ID, int UserID)
        {
            var configs = _IUserConfig.GetUserConfig(UserID, "EngineeringFollow");

            if (configs != null)
            {
                return configs.Count(c => c.ConfigKey == ID.ToString()) > 0;
            }

            return false;
        }

        public List<BizObject> Get(PageQueryParam PageParam, int Deep = int.MaxValue)
        {
            var currentUser = PageParam.CurrentUser;

            var p_All = _IPMPermissionCheck.CheckObjAll(currentUser);
            var p_Dept = _IPMPermissionCheck.CheckObjDept(currentUser);

            var viewLevl = ObjectViewLevel.全部;

            if (p_All != PermissionStatus.Reject)
            {
                // 可以查看全部进度
            }
            else if (p_Dept != PermissionStatus.Reject)
            {
                // 可以查看部门工程进度
                viewLevl = ObjectViewLevel.部门;
                _DeptUsers = _IDepartment.GetMyDeptUsers(currentUser);
            }
            else
            {
                // 查看与自己相关工程的进度
                viewLevl = ObjectViewLevel.个人;
            }

            #region Filter

            Expression<Func<EngineeringEntity, bool>> expression = c => !c.IsDelete;
            if (PageParam.TextCondtion != null && !string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var value = filter.Value.ToString();

                if (value == "0" || string.IsNullOrEmpty(value))
                {
                    continue;
                }


                switch (filter.Key.ToString())
                {
                    case "Project.ID":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.ProjectID == intVal);

                            break;
                        }
                    case "Engineering.ID":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.ID == intVal);

                            break;
                        }
                    case "Engineering.Type":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Type == intVal);

                            break;
                        }
                    case "Engineering.Phase":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Phase == intVal);
                            break;
                        }
                    case "Engineering.VolLevel":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.VolLevel == intVal);
                            break;
                        }
                    case "Engineering.TaskType":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.TaskType == intVal);
                            break;
                        }
                    case "Engineering.Status":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Status == intVal);
                            break;
                        }
                    case "Engineering.Manager":
                        {
                            var intVal = int.Parse(value);
                            expression = expression.And(c => c.Manager == intVal);
                            break;
                        }
                    case "Engineering.DateRange":
                        {
                            var intVal = int.Parse(value);
                            switch (intVal)
                            {
                                case 0: break;
                                case 1: break;
                                case 2: break;
                                case 3: break;
                                case 4: break;
                                default:
                                    break;
                            }
                            break;
                        }

                    default:
                        break;
                }
            }
            #endregion

            var list = _DB.GetList(expression);

            var result = new List<BizObject>();
            var haspermission = false;
            foreach (var item in list)
            {
                var obj = new EngineeringInfo(item);

                haspermission = checkPermission(viewLevl, currentUser, obj);

                if (Deep > 1)
                {
                    setChildren(obj, PageParam, haspermission, viewLevl, currentUser, 2, Deep);
                }

                if (haspermission || (obj.Children != null && obj.Children.Count > 0))
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        private void setChildren(BizObject obj, PageQueryParam pageParam, bool hasPermission, ObjectViewLevel viewLevel, int currentUser, int currentDeep, int MaxDeep)
        {
            var children = obj.GetChildren(pageParam);

            if (children != null)
            {
                foreach (var item in children)
                {
                    if (!hasPermission)
                    {
                        // 验证子对象有没有权限
                        hasPermission = checkPermission(viewLevel, currentUser, item);
                    }

                    if (currentDeep < MaxDeep)
                    {
                        setChildren(item, pageParam, hasPermission, viewLevel, currentUser, currentDeep + 1, MaxDeep);
                    }

                    if (hasPermission || (item.Children != null && item.Children.Count > 0))
                    {
                        if (obj.Children == null)
                        {
                            obj.Children = new List<BizObject>();
                        }

                        obj.Children.Add(item);
                    }
                }
            }
        }

        private bool checkPermission(ObjectViewLevel viewLevl, int currentUser, BizObject obj)
        {
            var users = obj.GetParentMainUsers();

            users.AddRange(obj.GetChildrenMainUsers(true));

            switch (viewLevl)
            {
                case ObjectViewLevel.全部:
                    return true;
                case ObjectViewLevel.部门:
                    foreach (var u in users)
                    {
                        if (_DeptUsers.Contains(u))
                        {
                            return true;
                        }
                    }
                    return false;
                case ObjectViewLevel.个人:
                    foreach (var u in users)
                    {
                        if (currentUser == u)
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}
