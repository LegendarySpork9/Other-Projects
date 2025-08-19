using GitHubScraper.Converters;
using System.Configuration;

namespace GitHubScraper.Models
{
    // Stores the settings use by the application.
    internal static class AppSettingsModel
    {
        public static string Owner { get; set; } = ConfigurationManager.AppSettings["Owner"] ?? StandardValues.MissingValues.Owner;
        public static string[] Repositories { get; set; } = (ConfigurationManager.AppSettings["Repositories"] ?? StandardValues.MissingValues.Repositories).Split(',');
    }
}
