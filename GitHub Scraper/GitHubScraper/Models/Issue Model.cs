using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the issue.
    internal class IssueModel
    {
        public string? Repository { get; set; }
        public required long Id { get; set; }
        public required int Number { get; set; }
        public required string Title { get; set; }
        public UserModel? Assignee { get; set; }
        public string? Type { get; set; }
        public required string State { get; set; }
        public object? Pull_Request { get; set; }
        public required DateTime Created_At { get; set; }
        public DateTime? Closed_At { get; set; }
        public required List<LabelModel> Labels { get; set; } = [];
    }
}
