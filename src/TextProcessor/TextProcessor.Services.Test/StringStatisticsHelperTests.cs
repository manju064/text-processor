using NUnit.Framework;
using TextProcessor.Api.Models;
using TextProcessor.Services.Services.Statistics;

namespace TextProcessor.Services.Test
{
    [TestFixture]
    public class StringStatisticsHelperTests
    {
        [Test]
        public void TestSpaces()
        {
            var type = StatisticType.Space;

            var helper = new StringStatisticsHelper();
            var textToSearch = "This is text with lot   of   spaces     ";

            var statistic = helper.GetStatistics(type,textToSearch);

            Assert.IsNotNull(statistic);
            Assert.Greater(statistic.Count, 0);
            Assert.AreEqual(statistic.StatisticType, type);
        }

        [Test]
        public void TestHyphens()
        {
            var type = StatisticType.Hyphen;

            var helper = new StringStatisticsHelper();
            var textToSearch = "This is text with -- and more -- -";

            var statistic = helper.GetStatistics(type,textToSearch);

            Assert.IsNotNull(statistic);
            Assert.Greater(statistic.Count, 0);
            Assert.AreEqual(statistic.StatisticType, type);
        }

        [Test]
        public void TestTabs()
        {
            var type = StatisticType.Tab;

            var helper = new StringStatisticsHelper();
            var textToSearch = "This is text with   tabs and more  \t         ";

            var statistic = helper.GetStatistics(type,textToSearch);

            Assert.IsNotNull(statistic);
            Assert.Greater(statistic.Count, 0);
            Assert.AreEqual(statistic.StatisticType, type);
        }

        [Test]
        public void TestCommas()
        {
            var type = StatisticType.Comma;

            var helper = new StringStatisticsHelper();
            var textToSearch = "This is text with , , ,,,,,,";

            var statistic = helper.GetStatistics(type,textToSearch);

            Assert.IsNotNull(statistic);
            Assert.Greater(statistic.Count, 0);
            Assert.AreEqual(statistic.StatisticType, type);
        }
    }
}
