using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;
using Api.Framework.Core.Chat;
using DM.Base.Entity;
using BPM.DB;
using BPM.ProcessModel;

namespace DM.Base.Service
{
    /// <summary>
    /// UserTask 服务
    /// </summary>
    public partial class DMUserTaskService : IDMUserTaskService
    {
        private BaseRepository<UserTaskEntity> _DB;
        private DMContext _Context;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        [Dependency]
        public IBorrowService _IBorrowService { get; set; }

        public DMUserTaskService()
        {
            this._Context = new DMContext();
            this._DB = new BaseRepository<UserTaskEntity>(this._Context);
        }

        /// <summary>
        /// 档案审批任务
        /// </summary>
        /// <param name="PageParam"></param>
        /// <returns></returns>
        public PageSource<UserTaskInfo> GetArchiveTasks(PageQueryParam PageParam)
        {
            Expression<Func<UserTaskInfo, bool>> expression = c => c.Status == (int)UserTaskStatus.下达;

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

            var query = from ut in this._Context.UserTaskEntity

                        join op in this._Context.ObjectProcessEntity
                        on ut.ProcessID equals op.ProcessID

                        join bo in this._Context.ArchiveBorrowEntity
                        on new { ID = op.ObjectID, Key = op.ObjectKey } equals new { ID = bo.ID, Key = "ArchiveBorrow" }

                        where !ut.IsDelete

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

                            BorrowInfo = bo
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
            return this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete && t.Status == (int)UserTaskStatus.下达).First();
        }

        public int Add(UserTaskEntity UserTask)
        {
            this._DB.Add(UserTask);
           
            var lctDateStr = UserTask.LctDate.HasValue ? string.Format("预计完成时间：{0}", UserTask.LctDate.Value) : "";

            _NotifySrv.Send(new
            {
                Target = UserTask.UserID,
                Head = "借阅任务",
                Title = UserTask.Name,
                Content = UserTask.Time > 1 ? string.Format("第{0}次，{1}", UserTask.Time, lctDateStr) : lctDateStr,
            });
            return UserTask.ID;
        }

        public Guid TaskDone(int TaskID, string Note)
        {
            var task = this._DB.Get(TaskID);
            task.Status = (int)UserTaskStatus.完成;
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
                this._Context.Entry(task).State = System.Data.Entity.EntityState.Modified;
            }

            this._Context.SaveChanges();
        }

        public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

        public int Count(Expression<Func<UserTaskEntity, bool>> predicate)
        {
            return this._DB.Count(predicate);
        }

        public List<UserTaskEntity> GetTaskLog(Guid ProcessID)
        {
            return this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete).ToList();
        }

        public void ResetTaskUser(Guid ProcessID, List<TaskInfo> TaskUsers)
        {
            throw new NotImplementedException();
        }
    }
}
