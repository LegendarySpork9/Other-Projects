// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the logger service.
    public interface ILoggerService
    {
        void LogMessage(string level, string message);
    }
}
