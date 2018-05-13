using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextProcessor.Api.Models;
using TextProcessor.Services.Services.Sort;
using TextProcessor.Services.Services.Statistics;
using TextProcessor.Base.Enums;
using TextProcessor.Base.Extensions;
using TextProcessor.Base.Helpers;

namespace TextProcessor.Services.Services
{
    #region Interface
    /// <summary>
    /// Text processor services
    /// </summary>
    public interface ITextProcessorService
    {
        /// <summary>
        /// Get statistics of text
        /// </summary>
        /// <param name="statisticTypes"></param>
        /// <param name="textToProcess"></param>
        /// <returns></returns>
        Task<IList<Statistic>> GetStatistics(IList<StatisticType> statisticTypes, string textToProcess);

        /// <summary>
        /// Sort texts
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sortBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<string> Sort(string text, SortBy sortBy, OrderBy order);
    }
    #endregion

    #region Implementation
    public class TextProcessorService : ITextProcessorService
    {
        private readonly IStatisticsHelperFactory _statisticsHelperFactory;
        private readonly IComparatorFactory _comparatorFactory;

        public TextProcessorService(IStatisticsHelperFactory statisticsHelperFactory, IComparatorFactory comparatorFactory)
        {
            Guard.IsNotNull(statisticsHelperFactory, () => statisticsHelperFactory);
            Guard.IsNotNull(comparatorFactory, () => comparatorFactory);

            _statisticsHelperFactory = statisticsHelperFactory;
            _comparatorFactory = comparatorFactory;
        }

        public async Task<IList<Statistic>> GetStatistics(IList<StatisticType> statisticTypes, string textToProcess)
        {
            Guard.IsNotNullOrEmpty(textToProcess, () => textToProcess);

            var concurrentBag = new ConcurrentBag<Statistic>();

            await Task.Run(() => Parallel.ForEach(statisticTypes, new ParallelOptions(), (statisticType) =>
            {
                var result = _statisticsHelperFactory.GetComponentModelHelper(statisticType)
                    .GetStatistics(statisticType, textToProcess);

                if (result.Count > 0)
                    concurrentBag.Add(result);
            }));

            return concurrentBag?.ToList();
        }

        public async Task<string> Sort(string text, SortBy sortBy, OrderBy order)
        {
            Guard.IsNotNullOrEmpty(text, () => text);

            string[] words = text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            await Task.Run(() =>
            {
                var comparator = _comparatorFactory.GetComparator(sortBy);

                if (comparator is null)
                {
                    //TODO, refactor this into strategy pattern
                    switch (sortBy)
                    {
                        case SortBy.Alphabetical:
                        case SortBy.Numeric:
                            words = words.OrderBy(order).ToArray();
                            break;
                    }
                }
                else
                {
                    //Complex item comparision
                    Array.Sort(words, comparator);
                    words = Reverse(words).ToArray();
                }
            });

            //TODO, reverse the sorted list? or change comparator to receive order?
            IEnumerable<string> Reverse(string[] list)
            {
                if (order == OrderBy.Desc)
                    return list.Reverse();

                return list;
            }


            return String.Join(" ", words);
        }
    }

    #endregion

}
