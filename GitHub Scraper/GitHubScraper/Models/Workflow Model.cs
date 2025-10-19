using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the workflow.
    public class WorkflowModel
    {
        public required string Name { get; set; }
        public required List<WorkflowRunModel> WorkflowRuns { get; set; } = [];
    }
}
