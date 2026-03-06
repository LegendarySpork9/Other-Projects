// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the logger service.
    /// </summary>
    public interface ILoggerService
    {
        void LogMessage(string level, string message);
    }
}
