using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TextProcessor.Web.ApiControllers
{
    /// <summary>
    /// Base api controller
    /// </summary>
    [Controller]
    [Produces("application/json")]
    public abstract class Base : Controller
    {
        protected IConfiguration _configuration { get; }

        public Base(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
