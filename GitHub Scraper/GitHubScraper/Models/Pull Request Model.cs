using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the pull request.
    internal class PullRequestModel
    {
        public string? Repository { get; set; }
        public required int Number { get; set; }
        public required string Title { get; set; }
        public required UserModel Assignee { get; set; }
        public string? Type { get; set; }
        public required string State { get; set; }
        public required DateTime Created_At { get; set; }
        public DateTime? Closed_At { get; set; }
        public DateTime? Merged_At { get; set; }
        public required List<LabelModel> Labels { get; set; } = [];
    }
}
