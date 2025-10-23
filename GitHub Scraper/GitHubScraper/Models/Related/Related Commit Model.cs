// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Models.Related
{
    // Stores information about the related commit.
    public class RelatedCommitModel
    {
        public required UserModel Author { get; set; }
        public required UserModel Committer { get; set; }
        public required string Message { get; set; }
    }
}
