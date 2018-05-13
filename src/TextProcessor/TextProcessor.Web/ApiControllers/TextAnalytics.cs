using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TextProcessor.Api.Models;
using TextProcessor.Services.Services;
using TextProcessor.Base.Enums;
using TextProcessor.Base.Helpers;
using TextProcessor.Web.Infrastructure.Filters;
using TextProcessor.Web.Models.Response;

namespace TextProcessor.Web.ApiControllers
{
    /// <summary>
    /// Api end points for text analytics
    /// </summary>
    [ApiVersion("1.0")]
    public class TextAnalytics : Base
    {
        private readonly ITextProcessorService _textProcessor;
        public TextAnalytics(IConfiguration configuration, ITextProcessorService textProcessor)
            : base(configuration)
        {
            _textProcessor = textProcessor;
        }
        /// <summary>
        /// Get statistics of text
        /// </summary>
        /// <param name="inputTextModel">Text for statistics</param>
        /// <returns></returns>
        [HttpPost("api/texts/statistics")]
        [ProducesResponseType(typeof(ApiOkResponse<IList<Statistic>>), 200)]
        [ValidateModelState]
        public async Task<IActionResult> GetStatistics([FromBody] InputTextModel inputTextModel)
        {
            //Process all statistics types
            var statisticTypes = (IEnumerable<StatisticType>)Enum.GetValues(typeof(StatisticType));

            var statistic = await _textProcessor.GetStatistics(statisticTypes.ToList(), inputTextModel.Text);

            return Ok(new ApiOkResponse<IList<Statistic>>(statistic));
        }

        /// <summary>
        /// Sort the text
        /// </summary>
        /// <param name="inputTextModel"></param>
        /// <param name="sort">sort name</param>
        /// <param name="order">order by Asc or Desc</param>
        /// <returns></returns>
        [HttpPost("api/texts/sort")]
        [ProducesResponseType(typeof(ApiOkResponse<string>), 200)]
        [ValidateModelState]
        public async Task<IActionResult> Sort([FromBody] InputTextModel inputTextModel, [FromQuery]SortBy sort = default(SortBy),
            [FromQuery]OrderBy order = default(OrderBy))
        {
            var sortedText = await _textProcessor.Sort(inputTextModel.Text, sort, order);

            return Ok(new ApiOkResponse<string>(sortedText));
        }
    }
}
