using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TextProcessor.Api.Models;
using TextProcessor.Base.Helpers;

namespace TextProcessor.Services.Services.Statistics
{
    public class WordStatisticsHelper : BaseStatisticsHelper
    {
        public override Statistic GetStatistics(StatisticType statisticType, string textToSearch)
        {
            Guard.IsNotNullOrEmpty(textToSearch, () => textToSearch);

            int count = 0;

            switch (statisticType)
            {
                case StatisticType.Word:
                    count = Regex.Matches(new Regex("[^a-zA-Z0-9\\s]").Replace(textToSearch,""), @"[\S]+").Count;
                    break;
                case StatisticType.Paragraph:
                    count = textToSearch
                        .Split(
                            new[] { Environment.NewLine, "\n", "\r\n" },
                            StringSplitOptions.RemoveEmptyEntries)
                        .Count();
                    break;
            }

            return new Statistic(statisticType, count);
        }
    }
}
