using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using BPM.ProcessModel;
using PM.Base.Permission;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Chat;
using BPM.DB;

namespace PM.Base
{
    /// <summary>
    /// UserTask 服务
    /// </summary>
    public partial class PMUserTaskService : IPMUserTaskService,IUserTaskService
    {
        private BaseRepository<UserTaskEntity> _DB;
        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IPMPermissionCheck _IPMPermissionCheck { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        public PMUserTaskService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<UserTaskEntity>(this._PMContext);
        }


        public Dictionary<int,int> GetUserTaskCount(int UserID)
        {
            var q = from p in this._PMContext.UserTaskEntity
                    where p.UserID == UserID && p.Status == 1
                    group p by p.Source into g
                    select new
                    {
                        g.Key,
                        Count = g.Count()
                    };

            return q.ToDictionary(a => a.Key, a=> a.Count);
        }

        /// <summary>
        /// 生产任务
        /// </summary>
        /// <param name="PageParam"></param>
        /// <returns></returns>
        public PageSource<UserTaskInfo> GetProductionTasks(PageQueryParam PageParam)
        {
            Expression<Func<UserTaskInfo, bool>> expression = c => true;

            if (_IPMPermissionCheck.Check("DATA_AllUserWork", PageParam.CurrentUser.ToString()) != PermissionStatus.Reject)
            {
                // 全部查看权
            }
            else if (_IPMPermissionCheck.Check("DATA_DeptUserWork", PageParam.CurrentUser.ToString()) != PermissionStatus.Reject)
            {
                // 部门查看全
                var users = _IDepartment.GetMyDeptUsers(PageParam.CurrentUser).AsQueryable();
                expression = expression.And(c => users.Contains(c.UserID));
            }
            else
            {
                if (PageParam.FilterCondtion.ContainsKey("User"))
                {
                    PageParam.FilterCondtion["User"] = PageParam.CurrentUser;
                }
                else
                {
                    PageParam.FilterCondtion.Add("User", PageParam.CurrentUser);
                }
            }

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
                    case "User":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.UserID == id);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.Status == id);
                            }
                            else if (id == -1)
                            {
                                // 超期
                                expression = expression.And(c => c.LctDate.HasValue && c.LctDate < c.FinishDate);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from ut in this._PMContext.UserTaskEntity

                        join op in this._PMContext.ObjectProcessEntity
                        on ut.ProcessID equals op.ProcessID

                        join ev in this._PMContext.EngineeringVolumeEntity
                        on new { ID = op.ObjectID, Key = op.ObjectKey } equals new { ID = ev.ID, Key = "Volume" }

                        join en in this._PMContext.EngineeringEntity
                        on ev.EngineeringID equals en.ID

                        where !en.IsDelete && !ut.IsDelete && ut.Source == (int)TaskSource.生产

                        orderby ut.ReceiveDate descending
                        select new UserTaskInfo
                        {
                            ID = ut.ID,
                            FinishDate = ut.FinishDate,
                            UserID = ut.UserID,
                            Identity = ut.Identity,
                            Name = ut.Name,
                            ProcessID = ut.ProcessID,
                            ReceiveDate = ut.ReceiveDate,
                            LctDate = ut.LctDate,

                            Source = ut.Source,
                            Status = ut.Status,
                            Type = ut.Type,
                            Args = ut.Args,
                            IsDelete = ut.IsDelete,
                            Time = ut.Time,
                            TaskModelID = ut.TaskModelID,
                            Note = ut.Note,

                            Engineering = en,
                            Volume = ev
                        };

            query = query.Where(expression);
            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);
            PageParam.Count = result.TotalItemCount;

            //var source = new List<EngineeringVolumeInfo>();

            //foreach (var entity in result)
            //{
            //    source.Add(new EngineeringVolumeInfo(entity)
            //    {
            //        Volumes = this._PMContext.EngineeringVolumeEntity.Where(v => v.EngineeringID == entity.Engineering.ID && v.SpecialtyID == entity.Specialty.SpecialtyID).ToList()
            //    });
            //}

            return new PageSource<UserTaskInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        /// <summary>
        /// 提资任务
        /// </summary>
        /// <param name="PageParam"></param>
        /// <returns></returns>
        public PageSource<UserTaskInfo> GetProvideTasks(PageQueryParam PageParam)
        {
            Expression<Func<UserTaskInfo, bool>> expression = c => true;

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
                    case "User":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.UserID == id);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.Status == id);
                            }
                            else if (id == -1)
                            {
                                // 超期
                                expression = expression.And(c => c.LctDate.HasValue && c.LctDate < c.FinishDate);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from ut in this._PMContext.UserTaskEntity

                        join op in this._PMContext.ObjectProcessEntity
                        on ut.ProcessID equals op.ProcessID

                        join ev in this._PMContext.EngineeringSpecialtyProvideEntity
                        on new { ID = op.ObjectID, Key = op.ObjectKey } equals new { ID = ev.ID, Key = "EngineeringSpecialtyProvide" }

                        join en in this._PMContext.EngineeringEntity
                        on ev.EngineeringID equals en.ID

                        where !en.IsDelete && !ut.IsDelete && ut.Source == (int)TaskSource.提资

                        orderby ut.ReceiveDate descending
                        select new UserTaskInfo
                        {
                            ID = ut.ID,
                            FinishDate = ut.FinishDate,
                            UserID = ut.UserID,
                            Identity = ut.Identity,
                            Name = ut.Name,
                            ProcessID = ut.ProcessID,
                            ReceiveDate = ut.ReceiveDate,
                            Source = ut.Source,
                            Status = ut.Status,
                            Type = ut.Type,
                            Args = ut.Args,
                            IsDelete = ut.IsDelete,
                            TaskModelID = ut.TaskModelID,
                            Time = ut.Time,

                            Engineering = en,
                            SpecialtyProvide = ev
                        };

            query = query.Where(expression);

            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            return new PageSource<UserTaskInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        /// <summary>
        /// 表单任务
        /// </summary>
        /// <param name="PageParam"></param>
        /// <returns></returns>
        public PageSource<UserTaskInfo> GetFormTasks(PageQueryParam PageParam)
        {
            Expression<Func<UserTaskInfo, bool>> expression = c => c.Source == (int)TaskSource.表单 && c.Status == (int)TaskStatus.下达;

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
                    case "User":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.UserID == id);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.Status == id);
                            }
                            else if (id == -1)
                            {
                                // 超期
                                expression = expression.And(c => c.LctDate.HasValue && c.LctDate < c.FinishDate);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from ut in this._PMContext.UserTaskEntity

                        join op in this._PMContext.ObjectProcessEntity
                        on ut.ProcessID equals op.ProcessID

                        where !ut.IsDelete && ut.Source == (int)TaskSource.表单

                        orderby ut.ReceiveDate descending
                        select new UserTaskInfo
                        {
                            ID = ut.ID,
                            FinishDate = ut.FinishDate,
                            UserID = ut.UserID,
                            Identity = ut.Identity,
                            Name = ut.Name,
                            ProcessID = ut.ProcessID,
                            ReceiveDate = ut.ReceiveDate,
                            Source = ut.Source,
                            Status = ut.Status,
                            Type = ut.Type,
                            Args = ut.Args,
                            IsDelete = ut.IsDelete,
                            TaskModelID = ut.TaskModelID,
                            Time = ut.Time,

                            ObjectKey = op.ObjectKey,
                            ObjectID = op.ObjectID,
                        };

            query = query.Where(expression);

            var result = query.AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            return new PageSource<UserTaskInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public UserTaskEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public UserTaskEntity GetCurrentTask(Guid ProcessID)
        {
            return this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete && t.Status == (int)TaskStatus.下达).First();
        }

        public int Add(UserTaskEntity UserTask)
        {
            this._DB.Add(UserTask);
            var head = Enum.GetName(typeof(TaskSource), UserTask.Type);
            var lctDateStr = UserTask.LctDate.HasValue ? string.Format("预计完成时间：{0}", UserTask.LctDate.Value) : "";

            _NotifySrv.Send(new
            {
                Target = UserTask.UserID,
                Head = head + "任务",
                Title = UserTask.Name,
                Content = UserTask.Time > 1 ? string.Format("第{0}次，{1}", UserTask.Time, lctDateStr) : lctDateStr,
            });

            return UserTask.ID;
        }

        public Guid TaskDone(int TaskID, string Note)
        {
            var task = this._DB.Get(TaskID);
            task.Status = (int)TaskStatus.完成;
            task.FinishDate = DateTime.Now;
            task.Note = Note;
            this._DB.Edit(task);

            return task.Identity;
        }

        public void Update(int ID, UserTaskEntity UserTask)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(UserTask);

            this._DB.Edit(entity);
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDelete = true;
            this._DB.Edit(entity);
        }

        public void DeleteProcessTask(Guid ProcessID)
        {
            var tasks = this._DB.GetList(t => t.ProcessID == ProcessID);

            foreach (var task in tasks)
            {
                task.IsDelete = true;
                this._PMContext.Entry(task).State = System.Data.Entity.EntityState.Modified;
            }

            this._PMContext.SaveChanges();
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
                ObjectKey = "UserTask",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        /// <summary>
        /// 重设任务用户
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="TaskUsers"></param>
        public void ResetTaskUser(Guid ProcessID, List<TaskInfo> TaskUsers)
        {
            var tasks = this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete && t.Status == (int)TaskStatus.下达);

            foreach (var task in tasks)
            {
                var taskUser = TaskUsers.SingleOrDefault(t => t.ID == task.Identity.ToString());

                if (taskUser.User != task.UserID)
                {
                    task.UserID = taskUser.User;
                    this._PMContext.Entry(task).State = System.Data.Entity.EntityState.Modified;

                    var head = Enum.GetName(typeof(TaskSource), task.Type);
                    var lctDateStr = task.LctDate.HasValue ? string.Format("预计完成时间：{0}", task.LctDate.Value) : "";

                    _NotifySrv.Send(new
                    {
                        Target = taskUser.User,
                        Head = head + "任务",
                        Title = task.Name,
                        Content = task.Time > 1 ? string.Format("第{0}次，{1}", task.Time, lctDateStr) : lctDateStr,
                    });
                }
            }

            this._PMContext.SaveChanges();
        }

        public int Count(Expression<Func<UserTaskEntity, bool>> predicate)
        {
            return this._DB.Count(predicate);
        }

        public List<UserTaskEntity> GetTaskLog(Guid ProcessID)
        {
            return this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete).ToList();
        }

    }
}
