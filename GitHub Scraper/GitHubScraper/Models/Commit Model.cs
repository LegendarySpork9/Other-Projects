// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the commit.
    public class CommitModel
    {
        public string? Repository { get; set; }
        public string? Sha { get; set; }
        public required RelatedCommitModel Commit { get; set; }
    }
}
