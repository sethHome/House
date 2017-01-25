using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Microsoft.Practices.Unity;
using System.Linq;
using Api.Framework.Core.Attach;
using PM.Base.Permission;
using Api.Framework.Core.Organization;
using Api.Framework.Core.Permission;

namespace PM.Base
{
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public partial class ProjectService : IProjectService, ITopBizObject
    {
        [Dependency]
        public ICustomerService _ICustomerService { get; set; }
        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }
        [Dependency]
        public IPMPermissionCheck _IPMPermissionCheck { get; set; }
        [Dependency]
        public IDepartment _IDepartment { get; set; }

        private BaseRepository<ProjectEntity> _DB;
        private PMContext _PMContext;
        private List<int> _DeptUsers;

        public ProjectService()
        {
            _PMContext = new PMContext();
            this._DB = new BaseRepository<ProjectEntity>(_PMContext);
        }

        public PageSource<ProjectInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<ProjectEntity, bool>> expression = c => c.IsDeleted == PageParam.IsDelete;

            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            #region FilterCondtion

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
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.ID == intVal);
                            }
                            break;
                        }
                    case "Kind":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Kind == intVal);
                            }
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
                    case "Vollevel":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.VolLevel == intVal);
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
                    case "Secretlevel":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.SecretLevel == intVal);
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

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);

            var source = new List<ProjectInfo>();

            var i = 1;
            foreach (var proj in pageSource)
            {
                source.Add(new ProjectInfo(proj)
                {
                    Index = (PageParam.PageIndex - 1) * PageParam.PageSize + i++,
                    Customer = _ICustomerService.Get(proj.CustomerID),
                    Engineerings = _PMContext.EngineeringEntity.Where(e => !e.IsDelete && e.ProjectID == proj.ID).ToList()
                });
            }

            return new PageSource<ProjectInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public ProjectEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public int Add(ProjectInfo Project)
        {
            var proj = new ProjectEntity(Project);
            proj.IsDeleted = false;
            this._DB.Add(proj);

            //foreach (var attachID in Project.AttachIDs)
            //{
            //    AddAttach(proj.ID, attachID);
            //}

            return proj.ID;
        }

        public void AddAttach(int ProjID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "Project",
                ObjectID = ProjID,
                AttachID = AttachID
            });
        }

        public void Update(int ID, ProjectEntity Project)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(Project);

            this._DB.Edit(entity);
        }

        public void BackUp(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                var entity = this._DB.Get(int.Parse(id));
                entity.IsDeleted = false;
                this._DB.Edit(entity);
            }
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDeleted = true;
            this._DB.Edit(entity);

            //this._DB.Delete(ID);
        }

        public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

        public List<ProjectEntity> GetSource(string number, string name)
        {
            Expression<Func<ProjectEntity, bool>> expression = c => !c.IsDeleted;

            if (!string.IsNullOrEmpty(number))
            {
                expression = expression.And(p => p.Number.Contains(number));
            }

            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(p => p.Name.Contains(name));
            }

            return _DB.GetList(expression).ToList();
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
            else {
                // 查看与自己相关工程的进度
                viewLevl = ObjectViewLevel.个人;
            }

            Expression<Func<ProjectEntity, bool>> expression = p => !p.IsDeleted;

            var list = _DB.GetList(expression);

            var result = new List<BizObject>();
            var haspermission = false;
            foreach (var item in list)
            {
                var obj = new ProjectInfo(item);

                haspermission = checkPermission(viewLevl, currentUser, obj.GetMainUsers());

                if (Deep > 1 ) {
                    setChildren(obj, PageParam, haspermission, viewLevl, currentUser, 2,Deep);
                }

                if (haspermission || (obj.Children != null && obj.Children.Count > 0))
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        private void setChildren(BizObject obj, PageQueryParam pageParam, bool hasPermission, ObjectViewLevel viewLevel,int currentUser,int currentDeep, int MaxDeep)
        {
            var children = obj.GetChildren(pageParam);

            if (children != null)
            {
                foreach (var item in children)
                {
                    if (!hasPermission)
                    {
                        // 验证子对象有没有权限
                        hasPermission = checkPermission(viewLevel, currentUser, item.GetMainUsers());
                    }

                    if (currentDeep < MaxDeep) {
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

        private bool checkPermission(ObjectViewLevel viewLevl, int currentUser, params int[] users)
        {
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
