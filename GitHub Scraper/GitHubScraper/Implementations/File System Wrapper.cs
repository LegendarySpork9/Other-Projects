// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;

namespace GitHubScraper.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        /// <summary>
        /// Returns the text in a given file.
        /// </summary>
        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);
    }
}
