// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the DateTime object.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime DefaultDate { get; }
    }
}
