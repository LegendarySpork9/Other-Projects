using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the commit.
    internal class CommitModel
    {
        public string? Repository { get; set; }
        public required UserModel Author { get; set; }
        public required UserModel Committer { get; set; }
        public required string Message { get; set; }
    }
}
