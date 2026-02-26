// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Abstractions
{
    // Interface for the file system operations.
    public interface IFileSystem
    {
        string ReadAllText(string path);
    }
}
