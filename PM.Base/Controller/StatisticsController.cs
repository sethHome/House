
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using System.Linq;

namespace PM.Base
{
    public class StatisticsController : ApiController
    {
        [Dependency]
        public IStatistics _IStatistics { get; set; }

        #region engineering
        
        [Route("api/v1/statistics/engineering/by/{Category}")]
        [HttpGet]
        public List<StatisticsInfo> GetEngineeringCount(string Category, int year = 0)
        {
            return _IStatistics.GetEngineeringCount(Category, year);
        }

        [Route("api/v1/statistics/engineering")]
        [HttpGet]
        public List<StatisticsInfo> GetYearEngineeringCount()
        {
            return _IStatistics.GetYearEngineeringCount();
        }

        [Route("api/v1/statistics/engineering/{Year}")]
        [HttpGet]
        public List<StatisticsInfo> GetMonthEngineeringCount(int Year)
        {
            return _IStatistics.GetMonthEngineeringCount(Year);
        }
        #endregion

        #region contract

        [Route("api/v1/statistics/contract/money")]
        [HttpGet]
        public List<StatisticsInfo> GetYearContractMoney(int Type = 2)
        {
            return _IStatistics.GetYearContractMoney(Type);
        }

        [Route("api/v1/statistics/contract/money/{Year}")]
        [HttpGet]
        public List<StatisticsInfo> GetMonthContractMoney(int Year, int Type = 2)
        {
            return _IStatistics.GetMonthContractMoney(Type, Year);
        }

        [Route("api/v1/statistics/contract/count")]
        [HttpGet]
        public List<StatisticsInfo> GetYearContractCount()
        {
            return _IStatistics.GetYearContractCount();
        }

        [Route("api/v1/statistics/contract/count/{Year}")]
        [HttpGet]
        public List<StatisticsInfo> GetMonthContractCount(int Year)
        {
            return _IStatistics.GetMonthContractCount(Year);
        }

        #endregion

        #region my

        [Token]
        [Route("api/v1/statistics/my")]
        [HttpGet]
        public MyStatistics GetMyStatistics()
        {
            return _IStatistics.GetUserStatistics(int.Parse(base.User.Identity.Name));
        }
        #endregion

        [Route("api/v1/statistics")]
        [Route("api/v1/statistics/my")]
        [Route("api/v1/statistics/engineering")]
        [Route("api/v1/statistics/engineering/{Year}")]
        [Route("api/v1/statistics/engineering/by/{Category}")]

        [Route("api/v1/statistics/contract/money")]
        [Route("api/v1/statistics/contract/money/{Year}")]
        [Route("api/v1/statistics/contract/count")]
        [Route("api/v1/statistics/contract/count/{Year}")]
        [HttpOptions]
        public void Option()
        { }
    }
}
