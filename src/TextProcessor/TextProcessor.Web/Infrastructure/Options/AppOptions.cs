namespace TextProcessor.Web.Infrastructure.Options
{
    /// <summary>
    /// The configuration holder class
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// Application version details
        /// </summary>
        public Application Application { get; set; }
    }

    /// <summary>
    /// Application name and version
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get; set; }
    }
}
