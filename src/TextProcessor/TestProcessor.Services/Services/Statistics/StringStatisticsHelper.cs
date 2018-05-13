using System;
using System.Linq;
using System.Text.RegularExpressions;
using TextProcessor.Api.Models;
using TextProcessor.Base.Extensions;
using TextProcessor.Base.Helpers;

namespace TextProcessor.Services.Services.Statistics
{
    /// <summary>
    /// This class helps to get the statics for given char
    /// </summary>
    public class StringStatisticsHelper : BaseStatisticsHelper
    {
        public override Statistic GetStatistics(StatisticType statisticType, string textToSearch)
        {
            Guard.IsNotNullOrEmpty(textToSearch, ()=> textToSearch);

            string match = statisticType.GetStringValue();

            var count = Regex.Matches(textToSearch, $"[{match}+]", RegexOptions.IgnoreCase).Count;

            return new Statistic(statisticType, count);
        }
    }
}
