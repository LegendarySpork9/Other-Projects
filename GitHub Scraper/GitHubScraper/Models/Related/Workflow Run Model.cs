// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Models.Related
{
    // Stores information about the workflow run.
    public class WorkflowRunModel
    {
        public string? RepositoryName { get; set; }
        public required long Id { get; set; }
        public required int Run_Number { get; set; }
        public required UserModel Actor { get; set; }
        public required string Name { get; set; }
        public required string Display_Title { get; set; }
        public required string Event { get; set; }
        public required string Status { get; set; }
        public required string Conclusion { get; set; }
        public required DateTime Created_At { get; set; }
        public required DateTime Updated_At { get; set; }
    }
}
