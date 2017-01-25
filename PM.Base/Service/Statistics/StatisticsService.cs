using Api.Framework.Core.Attach;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class StatisticsService : IStatistics
    {
        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        private PMContext _PMContext;

        public StatisticsService()
        {
            this._PMContext = new PMContext();
        }

        public MyStatistics GetUserStatistics(int UserID)
        {
            var result = new MyStatistics();

            // 获取用户参与的工程数,卷册数量
            var query = from v in _PMContext.EngineeringVolumeEntity
                        where v.Designer == UserID && !v.IsDelete
                        group v by v.EngineeringID into g
                        select new StatisticsInfo()
                        {
                           Key = g.Key,
                           Count = g.Count()
                        };

            result.VolumeCount = query.Sum(q => q.Count);
            result.EngCount = query.Count();

            // 获取总的工程数，卷册数
            result.TotalEngCount = _PMContext.EngineeringEntity.Count(e => !e.IsDelete);
            result.TotalVolumeCount = _PMContext.EngineeringVolumeEntity.Count(v => !v.IsDelete);

            // 今年的工程数，卷册数
            var thisYear = DateTime.Now.Year;
            result.ThisYear = thisYear;
            result.ThisYearEngCount = _PMContext.EngineeringEntity.Count(e => !e.IsDelete && e.CreateDate.Year == thisYear);
            result.ThisYearVolumeCount = _PMContext.EngineeringVolumeEntity.Count(e => !e.IsDelete && e.Designer == UserID && e.StartDate.HasValue && e.StartDate.Value.Year == thisYear);

            // 上传的设计文件，收资文件，提资文件数量
            var objKeys = new List<string>() { "Volume", "EngineeringResource", "EngineeringSpecialtyProvide" }.AsQueryable();
            var attachYearsCount = _IObjectAttachService.GetMyYearAttachCount(UserID, objKeys);

            result.TotalAttachCount = _IObjectAttachService.GetAttachCount(objKeys);
            result.TotalMyAttachCount = 0;
            result.ThisYearMyAttachCount = 0;

            foreach (var item in attachYearsCount)
            {
                result.TotalMyAttachCount += item.Count;
                if (item.Year == thisYear)
                {
                    result.ThisYearMyAttachCount = item.Count;
                }
            }

            return result;
        }

        public List<StatisticsInfo> GetEngineeringCount(string GroupBy, int Year = 0)
        {
            if (string.IsNullOrEmpty(GroupBy)) throw new ArgumentNullException("GroupBy");

            var param = Expression.Parameter(typeof(EngineeringEntity));
            Expression<Func<EngineeringEntity, int>> keySelector = Expression.Lambda<Func<EngineeringEntity, int>>
            (
                Expression.Property(param, GroupBy),
                param
            );

            var count = _PMContext.EngineeringEntity.Count(e => !e.IsDelete && (Year == 0 || e.CreateDate.Year == Year));

            return _PMContext.EngineeringEntity.Where(e => !e.IsDelete && (Year == 0 || e.CreateDate.Year == Year))
                .GroupBy(keySelector).Select(p => new StatisticsInfo() { Key = p.Key, Count = p.Count(), TotalCount = count }).ToList();

        }

        /// <summary>
        /// 年度工程数量统计
        /// </summary>
        /// <returns></returns>
        public List<StatisticsInfo> GetYearEngineeringCount()
        {
            var query = from v in _PMContext.EngineeringEntity
                        where !v.IsDelete
                        group v by v.CreateDate.Year into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Count = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 月度工程数量统计
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public List<StatisticsInfo> GetMonthEngineeringCount(int Year)
        {
            var query = from v in _PMContext.EngineeringEntity
                        where !v.IsDelete && v.CreateDate.Year == Year
                        group v by v.CreateDate.Month into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Count = g.Count()
                        };
            return query.ToList();
        }

        /// <summary>
        /// 年度合同数量
        /// </summary>
        /// <returns></returns>
        public List<StatisticsInfo> GetYearContractCount()
        {
            var query = from v in _PMContext.ContractEntity
                        where !v.IsDelete
                        group v by v.CreateDate.Year into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Count = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 年度合同金额
        /// </summary>
        /// <returns></returns>
        public List<StatisticsInfo> GetYearContractMoney(int Type)
        {
            var query = from v in _PMContext.ContractPayeeEntity
                        where !v.IsDelete && v.Type == Type
                        group v by v.Date.Year into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Money = g.Sum(a => a.Fee)
                        };

            return query.ToList();
        }

        /// <summary>
        /// 月度合同数量
        /// </summary>
        /// <returns></returns>
        public List<StatisticsInfo> GetMonthContractCount(int Year)
        {
            var query = from v in _PMContext.ContractEntity
                        where !v.IsDelete && v.CreateDate.Year == Year
                        group v by v.CreateDate.Month into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Count = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 月度合同金额
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public List<StatisticsInfo> GetMonthContractMoney(int Type,int Year)
        {
            var query = from v in _PMContext.ContractPayeeEntity
                        where !v.IsDelete && v.Type == Type && v.Date.Year == Year
                        group v by v.Date.Month into g
                        select new StatisticsInfo()
                        {
                            Key = g.Key,
                            Money = g.Sum(a => a.Fee)
                        };
            return query.ToList();
        }
    }
}
