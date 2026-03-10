// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the file system operations.
    /// </summary>
    public interface IFileSystem
    {
        Task<string> ReadAllTextAsync(string path);
    }
}
