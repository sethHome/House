using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolumeCheck 服务
    /// </summary>
    public partial class EngineeringVolumeCheckService : IEngineeringVolumeCheckService
    {
        private BaseRepository<EngineeringVolumeCheckEntity> _DB;
        private PMContext _PMContext;

        public EngineeringVolumeCheckService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<EngineeringVolumeCheckEntity>(this._PMContext);
        }

        public PageSource<EngineeringVolumeCheckEntity> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<EngineeringVolumeCheckEntity, bool>> expression = c => !c.IsDelete;

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
                    case "VolumeID":
                        {
                            var id = int.Parse(val);
                            expression = expression.And(c => c.VolumeID == id);
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            //var source = new List<EngineeringVolumeCheckInfo>();

            //foreach (var entity in pageSource)
            //{
            //    source.Add(new EngineeringVolumeCheckInfo(entity)
            //    {

            //    });
            //}

            return new PageSource<EngineeringVolumeCheckEntity>()
            {
                Source = pageSource,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public EngineeringVolumeCheckEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public EngineeringVolumeCheckEntity Add(EngineeringVolumeCheckEntity Entity)
        {
            Entity.Date = DateTime.Now;
            Entity.IsCorrect = false;
            Entity.IsPass = false;
            Entity.IsDelete = false;

            this._DB.Add(Entity);
            return Entity;
        }

        public void Update(int ID, EngineeringVolumeCheckEntity EngineeringVolumeCheck)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(EngineeringVolumeCheck);

            this._DB.Edit(entity);
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDelete = true;
            this._DB.Edit(entity);
        }

        public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

        public List<VolumeCheckGroupInfo> GetVolumeCheckStatistics(int ID)
        {
            var totalCount = _DB.Count(c => c.VolumeID == ID && !c.IsDelete);

            var groupQuery =
                from c in _PMContext.EngineeringVolumeCheckEntity
                where c.VolumeID == ID && !c.IsDelete
                group c by c.Type into g
                select new VolumeCheckGroupInfo()
                {
                    CheckType = g.Key,
                    CheckCount = g.Count(),
                    //Percent = g.Count() / totalCount * 100
                };

            var result = groupQuery.ToList();
            foreach (var item in result)
            {
                item.Percent = (double)item.CheckCount / (double)totalCount * 100;
            }
            return result;
        }

        public List<EngineeringVolumeCheckEntity> GetVolumeCheckList(int VolumeID)
        {
            return this._DB.GetList(c => c.VolumeID == VolumeID && !c.IsDelete).ToList();
        }
    }
}
