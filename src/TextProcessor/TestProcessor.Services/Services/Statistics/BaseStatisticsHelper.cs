using TextProcessor.Api.Models;

namespace TextProcessor.Services.Services.Statistics
{
    public abstract class BaseStatisticsHelper 
    {
        public abstract Statistic GetStatistics(StatisticType statisticType, string textToSearch);
    }
}
