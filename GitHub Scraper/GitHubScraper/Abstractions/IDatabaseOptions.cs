// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the database options.
    /// </summary>
    public interface IDatabaseOptions
    {
        string ConnectionString { get; }
        string SQLFiles { get; }
    }
}
