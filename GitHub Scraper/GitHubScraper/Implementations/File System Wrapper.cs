// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;

namespace GitHubScraper.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        /// <summary>
        /// Returns the text in a given file.
        /// </summary>
        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}
