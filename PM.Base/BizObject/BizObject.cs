using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    [KnownType(typeof(ProjectInfo))]
    [KnownType(typeof(EngineeringInfo))]
    [KnownType(typeof(EngineeringSpecialtyInfo))]
    [KnownType(typeof(EngineeringVolumeInfo))]
    [KnownType(typeof(EngineeringResourceInfo))]
    [KnownType(typeof(FormChangeInfo))]
    [KnownType(typeof(EngineeringVolumeCheckForm))]
    /// <summary>
    /// 一个描述业务对象树形结构的抽象
    /// </summary>
    public abstract class BizObject
    {
        public BizObject()
        {
            this.HasTask = false;
            this.JoinPermissionCheckUser = true;
        }

        private Int32 _ID;

        public Int32 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                this.ObjectID = string.Format("{0}_{1}", this.ObjectKey, value);
            }
        }

        public string ObjectID { get; set; }

        public string ObjectKey { get; set; }

        public string ObjectText { get; set; }

        /// <summary>
        /// 是否有流程任务
        /// </summary>
        public bool HasTask { get; set; }

        /// <summary>
        /// 当前用户是否参与上下级权限判断
        /// </summary>
        public bool JoinPermissionCheckUser { get; set; }

        /// <summary>
        /// 任务总数
        /// </summary>
        public int TaskCount
        {
            get
            {
                if (this.HasTask)
                {
                    return this.Tasks != null ? this.Tasks.Count : 0;
                }
                else if (this.Children != null)
                {
                    return this.Children.Sum(c => c.TaskCount);
                }

                return 0;
            }
        }

        /// <summary>
        /// 已完成任务数量
        /// </summary>
        public int TaskDoneCount
        {
            get
            {
                if (this.HasTask)
                {
                    return this.Tasks != null ? this.Tasks.Count(t => t.Status == (int)TaskStatus.完成) : 0;
                }
                else if (this.Children != null)
                {
                    return this.Children.Sum(c => c.TaskDoneCount);
                }

                return 0;
            }
        }

        /// <summary>
        /// 任务进度
        /// </summary>
        public double TaskProcessPercent
        {
            get
            {
                if (this.TaskCount > 0)
                {
                    return (double)this.TaskDoneCount / (double)this.TaskCount * 100;
                }

                return 0;
            }
        }

        public List<BPM.DB.BPMTaskInstanceEntity> Tasks { get; set; }

        public List<BizObject> Children { get; set; }

        public abstract BizObject GetParent();

        public abstract List<BizObject> GetChildren(PageQueryParam PageParam);

        public abstract int[] GetMainUsers();

        public virtual List<BPM.DB.BPMTaskInstanceEntity> GetTasks()
        {
            return null;
        }

        public List<int> GetChildrenMainUsers(bool IsPermissionUser = false)
        {
            var result = _GetChildrenMainUsers(this, IsPermissionUser);

            return result.Distinct().ToList();
        }

        /// <summary>
        /// 递归获取所有父级相关的用户
        /// </summary>
        /// <returns></returns>
        public List<int> GetParentMainUsers(bool IsPermissionUser = false)
        {
            var result = _GetParentMainUsers(this, IsPermissionUser);

            return result.Distinct().ToList();
        }


        private List<int> _GetParentMainUsers(BizObject obj, bool IsPermissionUser)
        {
            var result = new List<int>();

            if (!IsPermissionUser || obj.JoinPermissionCheckUser)
            {
                var users = obj.GetMainUsers();

                if (users != null)
                {
                    result.AddRange(users);
                }
            }

            // 如果获取用户是为了参与权限判断且有流程任务的话，流程节点上的用户也算入其中
            if (this.HasTask && IsPermissionUser)
            {
                var tasks = GetTasks();

                foreach (var t in tasks)
                {
                    if (t.UserID > 0)
                    {
                        result.Add(t.UserID);
                    }

                    if (!string.IsNullOrEmpty(t.Users))
                    {
                        result.AddRange(t.Users.Split(',').Select(u => int.Parse(u)));
                    }
                }
            }

            var parentObj = obj.GetParent();

            if (parentObj != null)
            {
                var parentUsers = _GetParentMainUsers(parentObj, IsPermissionUser);

                if (parentUsers != null)
                {
                    result.AddRange(parentUsers);
                }
            }

            return result;
        }

        private List<int> _GetChildrenMainUsers(BizObject obj, bool IsPermissionUser)
        {
            var result = new List<int>();

            if (!IsPermissionUser || obj.JoinPermissionCheckUser) {
                var users = obj.GetMainUsers();

                if (users != null)
                {
                    result.AddRange(users);
                }
            }

            // 如果获取用户是为了参与权限判断且有流程任务的话，流程节点上的用户也算入其中
            if (this.HasTask && IsPermissionUser)
            {
                var tasks = GetTasks();

                foreach (var t in tasks)
                {
                    if (t.UserID > 0)
                    {
                        result.Add(t.UserID);
                    }

                    if (!string.IsNullOrEmpty(t.Users))
                    {
                        result.AddRange(t.Users.Split(',').Select(u => int.Parse(u)));
                    }
                }
            }

            var childrenObjs = obj.GetChildren(null);

            if (childrenObjs != null)
            {
                foreach (var o in childrenObjs)
                {
                    var parentUsers = _GetChildrenMainUsers(o, IsPermissionUser);

                    if (parentUsers != null)
                    {
                        result.AddRange(parentUsers);
                    }
                }
            }

            return result;
        }
    }
}
