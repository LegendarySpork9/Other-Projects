// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Models;

namespace GitHubScraper.Implementations
{
    public class DatabaseOptionsProvider : IDatabaseOptions
    {
        // Returns the ConnectionString from AppSettings.
        public string ConnectionString => AppSettingsModel.ConnectionString;

        // Returns the SQLFiles from AppSettings.
        public string SQLFiles => AppSettingsModel.SQLFiles;
    }
}
