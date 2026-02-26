// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the database options.
    public interface IDatabaseOptions
    {
        string ConnectionString { get; }
        string SQLFiles { get; }
    }
}
