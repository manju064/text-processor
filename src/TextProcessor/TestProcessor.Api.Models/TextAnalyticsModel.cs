using System.Collections.Generic;
using TextProcessor.Base.Attributes;

namespace TextProcessor.Api.Models
{
    public class Statistic
    {
        public StatisticType StatisticType { get; }
        public int Count { get;  }

        public Statistic(StatisticType statisticType, int count)
        {
            StatisticType = statisticType;
            Count = count;
        }
    }

    public enum StatisticType
    {
        [StringValue("-")]
        Hyphen,
        [StringValue(",")]
        Comma,
        [StringValue("\t")]
        Tab,
        [StringValue(" ")]
        Space,
        Word,
        Paragraph
    }

  
}
