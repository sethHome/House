using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.DB
{
    public class BPMJoinSignTaskService
    {
        private BaseRepository<BPMJoinSignTaskEntity> _DB;
        private BPMContext _PMContext;

        public BPMJoinSignTaskService()
        {
            this._PMContext = new BPMContext();
            this._DB = new BaseRepository<BPMJoinSignTaskEntity>(this._PMContext);
        }

        /// <summary>
        /// 给每个用户生成会签任务
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="Users"></param>
        public Dictionary<int, int> CreateTasks(string TaskID, string Users)
        {
            var result = new Dictionary<int, int>();

            var userIDs = Users.Split(',');
            foreach (var userID in userIDs)
            {
                var entity = new BPMJoinSignTaskEntity()
                {
                    TaskID = new Guid(TaskID),
                    Status = 1,
                    UserID = int.Parse(userID),
                    IsChecked = false
                };

                this._DB.Add(entity);

                result.Add(entity.ID, entity.UserID);
            }

            return result;

        }

        /// <summary>
        /// 检查会签任务是否都完成
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public bool CheckAllTaskDone(string TaskID)
        {
            return this._DB.Count(t => t.TaskID == new Guid(TaskID) && t.Status == 1) == 0;
        }

        /// <summary>
        /// 检查是否有会签失败的记录
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public bool CheckFailureSign(string TaskID)
        {
            var checkedEnittys = this._DB.GetList(t => t.TaskID == new Guid(TaskID) && !t.IsChecked);
            var result = false;

            foreach (var entity in checkedEnittys)
            {
                if (!entity.Result)
                {
                    result = true;
                }

                entity.IsChecked = true;
                this._PMContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }

            this._PMContext.SaveChanges();

            return result;
        }

        /// <summary>
        /// 检查任务是否有会签任务
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public bool Exists(string TaskID)
        {
            return this._DB.Count(t => t.TaskID == new Guid(TaskID) && t.Status == 1 ) > 0;
        }

        /// <summary>
        /// 会签任务完成
        /// </summary>
        /// <param name="JoinSignID"></param>
        /// <param name="result"></param>
        public void TaskDone(int JoinSignID, bool result)
        {
            var entity = this._DB.Get(JoinSignID);
            entity.FinishDate = DateTime.Now;
            entity.Result = result;
            entity.Status = 2;

            this._DB.Edit(entity);
        }
    }
}
