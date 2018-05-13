using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TextProcessor.Api.Models;
using TextProcessor.Services.Services.Statistics;

namespace TextProcessor.Services.Test
{
    [TestFixture]
    public class WordStatisticsHelperTests
    {
        [Test]
        public void TestWords()
        {
            var type = StatisticType.Word;

            var helper = new WordStatisticsHelper();
            var textToSearch = "one two three four five six   seven eight nine  -- --";

            var statistic = helper.GetStatistics(type, textToSearch);

            Assert.IsNotNull(statistic);
            Assert.AreEqual(statistic.Count, 9);
            Assert.AreEqual(statistic.StatisticType, type);
        }

        [Test]
        public void TestParagraphs()
        {
            var type = StatisticType.Paragraph;

            var helper = new WordStatisticsHelper();
            var textToSearch = "First paragraphs \n Second paragraph";

            var statistic = helper.GetStatistics(type, textToSearch);

            Assert.IsNotNull(statistic);
            Assert.AreEqual(statistic.Count, 2);
            Assert.AreEqual(statistic.StatisticType, type);
        }
    }
}
