// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the file system operations.
    /// </summary>
    public interface IFileSystem
    {
        string ReadAllText(string path);
    }
}
