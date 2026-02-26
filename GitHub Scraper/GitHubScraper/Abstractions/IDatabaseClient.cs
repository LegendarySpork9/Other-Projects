// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the database.
    public interface IDatabase
    {
        DateTime GetLastRunDate(string repository);
    }
}
