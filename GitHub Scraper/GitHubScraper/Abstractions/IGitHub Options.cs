// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the GitHub Options.
    public interface IGitHubOptions
    {
        string Owner { get; }
        string BearerToken { get; }
    }
}
