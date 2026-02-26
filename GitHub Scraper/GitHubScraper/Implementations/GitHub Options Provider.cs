// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Models;

namespace GitHubScraper.Implementations
{
    public class GitHubOptionsProvider : IGitHubOptions
    {
        // Returns the Owner from AppSettings.
        public string Owner => AppSettingsModel.Owner;

        // Returns the Bearer Token from AppSettings.
        public string BearerToken => AppSettingsModel.BearerToken;
    }
}
