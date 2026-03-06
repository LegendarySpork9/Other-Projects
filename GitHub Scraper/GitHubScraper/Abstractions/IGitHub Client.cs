// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Models;
using GitHubScraper.Models.Related;

namespace GitHubScraper.Abstractions
{
    /// <summary>
    /// Interface for the GitHub API.
    /// </summary>
    public interface IGitHubClient
    {
        Task<List<IssueModel>> GetIssues(string repository, DateTime lastRunDate);
        Task<List<CommitModel>> GetCommits(string repository, DateTime lastRunDate);
        Task<List<PullRequestModel>> GetPullRequests(string repository, DateTime lastRunDate);
        Task<List<WorkflowRunModel>> GetWorkflowRuns(string repository, string workflow, DateTime lastRunDate);
        Task<List<ReleaseModel>> GetReleases(string repository, DateTime lastRunDate);
    }
}
