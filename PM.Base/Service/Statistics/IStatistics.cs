using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public interface IStatistics
    {
        MyStatistics GetUserStatistics(int UserID);

        List<StatisticsInfo> GetEngineeringCount(string GroupBy, int Year = 0);

        List<StatisticsInfo> GetYearEngineeringCount();

        List<StatisticsInfo> GetMonthEngineeringCount(int Year);

        List<StatisticsInfo> GetYearContractMoney(int Type);

        List<StatisticsInfo> GetMonthContractMoney(int Type, int Year);

        List<StatisticsInfo> GetYearContractCount();

        List<StatisticsInfo> GetMonthContractCount(int Year);
        
    }
}
