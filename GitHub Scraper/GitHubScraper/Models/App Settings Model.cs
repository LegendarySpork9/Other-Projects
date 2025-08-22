using GitHubScraper.Converters;
using System.Configuration;

namespace GitHubScraper.Models
{
    // Stores the settings used by the application.
    internal static class AppSettingsModel
    {
        public static string Owner { get; set; } = ConfigurationManager.AppSettings["Owner"] ?? StandardValues.MissingValues.Owner;
        public static string[] Repositories { get; set; } = (ConfigurationManager.AppSettings["Repositories"] ?? StandardValues.MissingValues.Repositories).Split(',');
        public static string[] Workflows { get; set; } = (ConfigurationManager.AppSettings["Workflows"] ?? StandardValues.MissingValues.Workflows).Split(',');
        public static string BearerToken { get; set; } = ConfigurationManager.AppSettings["BearerToken"] ?? StandardValues.MissingValues.BearerToken;
    }
}
