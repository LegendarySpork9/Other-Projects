namespace GitHubScraper.Models
{
    // Stores the issue counts.
    public class IssueAggregateModel
    {
        public DateTime Date { get; set; }
        public int Created { get; set; }
        public int Solved { get; set; }
    }
}
