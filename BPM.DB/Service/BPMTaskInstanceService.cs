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

namespace BPM.DB
{
    /// <summary>
    /// BPMTaskInstance 服务
    /// </summary>
    public partial class BPMTaskInstanceService : IBPMTaskInstanceService
    {
        private BaseRepository<BPMTaskInstanceEntity> _DB;
        private BPMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public BPMTaskInstanceService()
        {
            this._PMContext = new BPMContext();
            this._DB = new BaseRepository<BPMTaskInstanceEntity>(this._PMContext);
        }

        public List<BPMTaskInstanceEntity> GetList(Expression<Func<BPMTaskInstanceEntity, bool>> Expression)
        {
            return this._DB.GetList(Expression).ToList().OrderBy(t => int.Parse(t.SourceID.Substring(1))).ToList();
        }

        public BPMTaskInstanceEntity Get(Guid ID)
        {
            return this._DB.Get(ID);
        }

        public string Add(BPMTaskInstanceInfo BPMTaskInstance)
        {
            var entity = new BPMTaskInstanceEntity(BPMTaskInstance);
            entity.IsDelete = false;
            this._DB.Add(entity);

            //foreach (var attachID in BPMTaskInstance.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID.ToString();
        }

        public void Update(string ID, BPMTaskInstanceEntity BPMTaskInstance)
        {
            var entity = this._DB.Get(new Guid(ID));

            entity.SetEntity(BPMTaskInstance);

            this._DB.Edit(entity);
        }

        public void TaskTurn(string ID, int UserID,string Users = "")
        {
            var entity = this._DB.Get(new Guid(ID));

            entity.TurnDate = DateTime.Now;
            entity.Status = 1;
            entity.UserID = UserID;
            entity.Users = Users;

            this._DB.Edit(entity);
        }
        public void TaskDone(string ID, int UserID)
        {
            var entity = this._DB.Get(new Guid(ID));

            entity.ExecuteDate = DateTime.Now;
            entity.Status = 2;
            entity.UserID = UserID;

            this._DB.Edit(entity);
        }

        public void SetTask(BPMTaskInstanceEntity BPMTaskInstance)
        {
            this._DB.Edit(BPMTaskInstance);
        }

        public void Delete(string ID)
        {
            this._DB.Delete(ID);
        }

        public void Deletes(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(id);
            }
        }

        private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "BPMTaskInstance",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        public void UpdateTaskUsers(Guid ProcessID, List<TaskInfo> TaskUsers)
        {
            var tasks = this._DB.GetList(t => t.ProcessID == ProcessID && !t.IsDelete && t.Type == 3);

            foreach (var task in TaskUsers)
            {
                var entity = tasks.SingleOrDefault(t => t.ID == new Guid(task.ID));

                entity.UserID = task.User;

                this._PMContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }

            this._PMContext.SaveChanges();
        }
    }
}
