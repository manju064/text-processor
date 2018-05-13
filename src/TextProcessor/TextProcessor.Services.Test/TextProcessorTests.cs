using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TextProcessor.Api.Models;
using TextProcessor.Services.Services;
using TextProcessor.Services.Services.Sort;
using TextProcessor.Services.Services.Statistics;
using TextProcessor.Base.Enums;

namespace TextProcessor.Services.Test
{
    [TestFixture]
    public class TextProcessorTests
    {
        #region Tests

        #region Statistics
        [Test]
        public void TestGetStatisticsForHyphens()
        {
            var textProcessor = GetTextProcessorServiceForStatistics();

            var types = new List<StatisticType>
            {
                StatisticType.Hyphen
            };

            var textToSearch = "This is a text with hyphens -- and more -- -";

            var statistics = textProcessor.GetStatistics(types, textToSearch).Result;

            Assert.IsNotNull(statistics);
            Assert.Greater(statistics.Count, 0);
            Assert.AreEqual(statistics?.Where(c => c.StatisticType == StatisticType.Hyphen).FirstOrDefault()?.Count, 5);
        }

        [Test]
        public void TestGetStatistics()
        {
            var textProcessor = GetTextProcessorServiceForStatistics();

            var types = new List<StatisticType>
            {
                StatisticType.Hyphen,
                StatisticType.Word
            };

            var textToSearch = "This is a text with hyphens and lot of words -- and more -- -";

            var statistics = textProcessor.GetStatistics(types, textToSearch).Result;

            Assert.IsNotNull(statistics);
            Assert.Greater(statistics.Count, 0);
            Assert.AreEqual(statistics?.Where(c => c.StatisticType == StatisticType.Hyphen).FirstOrDefault()?.Count, 5);
            Assert.AreEqual(statistics?.Where(c => c.StatisticType == StatisticType.Word).FirstOrDefault()?.Count, 12);
        }

        #endregion

        #region Sort
        public void TestAlphabeticalSortAsc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            var textToSort = "One Two Three Four";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphabetical, OrderBy.Asc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.Substring(0, 4) == "Four");
            Assert.IsTrue(sortedText.EndsWith("o", StringComparison.OrdinalIgnoreCase));
        }
        [Test]
        public void TestAlphabeticalSortDesc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            var textToSort = "One Two Three Four";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphabetical, OrderBy.Desc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.Substring(0, 3) == "Two");
            Assert.IsTrue(sortedText.EndsWith("r", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestNumericSortAsc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            var textToSort = "One 2 Three 5 a";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphabetical, OrderBy.Asc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.StartsWith("2"));
            Assert.IsTrue(sortedText.EndsWith("e"));
        }

        [Test]
        public void TestNumericSortDesc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            var textToSort = "One 2 Three 5 a";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphabetical, OrderBy.Desc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.StartsWith("T"));
            Assert.IsTrue(sortedText.EndsWith("2"));
        }
        [Test]
        public void TestAlphaNumericSortAsc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            //var textToSort = "z24 z2 z15 z1 z3, z20 z5 z11 z 21 z22";
            var textToSort = "image20.jpg image200.jpg image1.jpg image4.jpg";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphanumeric, OrderBy.Asc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.StartsWith("image1.jpg", StringComparison.Ordinal));
            Assert.IsTrue(sortedText.EndsWith("image200.jpg", StringComparison.Ordinal));
        }
        [Test]
        public void TestAlphaNumericSortDesc()
        {
            var textProcessorService = GetTextProcessorServiceForSort();

            //var textToSort = "z24 z2 z15 z1 z3, z20 z5 z11 z 21 z22";
            var textToSort = "image20.jpg image200.jpg image1.jpg image4.jpg";

            var sortedText = textProcessorService.Sort(textToSort, SortBy.Alphanumeric, OrderBy.Desc).Result;

            Assert.IsNotEmpty(sortedText);
            Assert.IsTrue(sortedText.StartsWith("image200.jpg", StringComparison.Ordinal));
            Assert.IsTrue(sortedText.EndsWith("image1.jpg", StringComparison.Ordinal));
        }
        #endregion

        #endregion

        #region Test Setup Helpers
        private ITextProcessorService GetTextProcessorServiceForStatistics()
        {
            var factoryMock = new Mock<IStatisticsHelperFactory>();
            factoryMock.Setup(s => s.GetComponentModelHelper(StatisticType.Hyphen))
                .Returns(new StringStatisticsHelper());
            factoryMock.Setup(s => s.GetComponentModelHelper(StatisticType.Word))
                .Returns(new WordStatisticsHelper());

            return new TextProcessorService(factoryMock.Object, Mock.Of<IComparatorFactory>());
        }

        private ITextProcessorService GetTextProcessorServiceForSort()
        {
            var factoryMock = new Mock<IComparatorFactory>();
            factoryMock.Setup(s => s.GetComparator(SortBy.Alphanumeric))
                .Returns(new AlphanumComparator());

            return new TextProcessorService(Mock.Of<IStatisticsHelperFactory>(), factoryMock.Object);
        }
        #endregion
    }
}
