using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the workflow run.
    internal class WorkflowRunModel
    {
        public string? Repository { get; set; }
        public required int Id { get; set; }
        public required int Run_Number { get; set; }
        public required UserModel Actor { get; set; }
        public required string Name { get; set; }
        public required string Display_Title { get; set; }
        public required string Event { get; set; }
        public required string Status { get; set; }
        public required string Conclusion { get; set; }
        public required DateTime Created_At { get; set; }
    }
}
