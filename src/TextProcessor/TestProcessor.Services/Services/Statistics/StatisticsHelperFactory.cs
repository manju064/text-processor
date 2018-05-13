using System;
using System.Collections.Generic;
using System.Text;
using TextProcessor.Api.Models;

namespace TextProcessor.Services.Services.Statistics
{
    #region Interafce
    /// <summary>
    /// Factory interface to provide statistics helper object
    /// </summary>
    public interface IStatisticsHelperFactory
    {
        BaseStatisticsHelper GetComponentModelHelper(StatisticType type);
    }

    #endregion

    /// <summary>
    /// Factory class for getting statistics helper class instances
    /// </summary>
    public class StatisticsHelperFactory : IStatisticsHelperFactory
    {
        private readonly IDictionary<StatisticType, Func<BaseStatisticsHelper>> _factory;

        public StatisticsHelperFactory(StringStatisticsHelper stringStatisticsHelper,
            WordStatisticsHelper wordStatisticsHelper) => _factory = new Dictionary<StatisticType, Func<BaseStatisticsHelper>>
            {
                {StatisticType.Comma, () => stringStatisticsHelper},
                {StatisticType.Hyphen, () => stringStatisticsHelper},
                {StatisticType.Space, () => stringStatisticsHelper},
                {StatisticType.Tab, () => stringStatisticsHelper},
                {StatisticType.Word, () => wordStatisticsHelper},
                {StatisticType.Paragraph, () => wordStatisticsHelper}
            };

        public BaseStatisticsHelper GetComponentModelHelper(StatisticType type)
        {
            return _factory[type]();
        }
    }
}
