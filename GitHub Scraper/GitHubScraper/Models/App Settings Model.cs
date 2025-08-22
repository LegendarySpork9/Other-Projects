using System.Configuration;

namespace GitHubScraper.Models
{
    // Stores the settings used by the application.
    internal static class AppSettingsModel
    {
        public static string? Owner { get; set; } = ConfigurationManager.AppSettings["Owner"];
        public static string? Repositories { get; set; } = ConfigurationManager.AppSettings["Repositories"];
        public static string? Workflows { get; set; } = ConfigurationManager.AppSettings["Workflows"];
        public static string? BearerToken { get; set; } = ConfigurationManager.AppSettings["BearerToken"];
    }
}
