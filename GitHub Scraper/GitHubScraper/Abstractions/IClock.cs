// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the DateTime object.
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime DefaultDate { get; }
    }
}
