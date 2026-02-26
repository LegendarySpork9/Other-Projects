// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;

namespace GitHubScraper.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        // Returns the text in a given file.
        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}
