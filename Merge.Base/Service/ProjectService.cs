using Api.Framework.Core;
using Api.Framework.Core.Attach;
using Api.Framework.Core.DBAccess;
using DM.Base.Entity;
using Merge.Base.Entitys;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Merge.Base.Service
{
    public class MProjectService : IMProjectService
    {
        private BaseRepository<ProjectEntity> _DB;
        private DBContext _DBContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public MProjectService()
        {
            _DBContext = new DBContext();

            this._DB = new BaseRepository<ProjectEntity>(_DBContext);
        }

        public ProjectEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public int Add(ProjectInfo Project)
        {
            var proj = new ProjectEntity(Project);
            proj.IsDelete = false;
            proj.CreateDate = DateTime.Now;
            this._DB.Add(proj);

            foreach (var item in Project.ProjSpecils)
            {
                item.ProjectID = proj.ID;
                item.IsMerge = false;
                _DBContext.ProjectSpecialtyEntity.Add(item);
            }

            _DBContext.SaveChanges();

            return proj.ID;
        }

        public PageSource<ProjectInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<ProjectEntity, bool>> expression = c => !c.IsDelete;

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
                    case "Number":
                        {
                            expression = expression.And(c => c.Number == val);
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
                    case "Area":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Area == intVal);
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
                    default:
                        break;
                }
            }
            #endregion


            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);

            var result = new List<ProjectInfo>();

            foreach (var item in pageSource)
            {
                result.Add(new ProjectInfo(item) {
                    ProjSpecils = _DBContext.ProjectSpecialtyEntity.Where(s => s.ProjectID == item.ID).ToList()
                });
            }

            return new PageSource<ProjectInfo>()
            {
                Source = result,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public List<ProjectInfo> GetMyTask(int UserID)
        {
            var projIDs = _DBContext.ProjectSpecialtyEntity.Where(s => s.Manager == UserID).Select(s => s.ProjectID);

            var pageSource = this._DBContext.ProjectEntity.Where(c => !c.IsDelete && projIDs.Contains(c.ID)).ToList();
            var result = new List<ProjectInfo>();

            foreach (var item in pageSource)
            {
                result.Add(new ProjectInfo(item)
                {
                    ProjSpecils = _DBContext.ProjectSpecialtyEntity.Where(s => s.Manager == UserID  && s.ProjectID == item.ID).ToList()
                });
            }

            return result;
        }

        public void Update(int ID, ProjectInfo Project)
        {
            var entity = this._DB.Get(ID);

            entity.Name = Project.Name;
            entity.Number = Project.Number;
            entity.Area = Project.Area;
            entity.Manager = Project.Manager;
            entity.Note = Project.Note;
            entity.DisableWord = Project.DisableWord;
            entity.Phase = Project.Phase;

            this._DB.Edit(entity);

            var specilIDs = Project.ProjSpecils.Select(s => s.SpecilID).AsQueryable();

            // 删除
            _DBContext.ProjectSpecialtyEntity.RemoveRange(_DBContext.ProjectSpecialtyEntity.Where(s => s.ProjectID == ID && !specilIDs.Contains(s.SpecilID)));

            // 所有专业
            var allSpecils = _DBContext.ProjectSpecialtyEntity.Where(s => s.ProjectID == ID);

            var allSpecilIDs = allSpecils.Select(s => s.SpecilID);

            foreach (var item in Project.ProjSpecils)
            {
                if (allSpecilIDs.Contains(item.SpecilID))
                {
                    // 更新
                    var specil = allSpecils.SingleOrDefault(s => s.SpecilID == item.SpecilID);

                    specil.Manager = item.Manager;
                    specil.Note = item.Note;

                    _DBContext.Entry(specil).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // 新增
                    item.ProjectID = ID;
                    _DBContext.ProjectSpecialtyEntity.Add(item);
                }
            }

            _DBContext.SaveChanges();
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDelete = true;
            this._DB.Edit(entity);

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

       
    }
}
