// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Models;

namespace GitHubScraper.Abstractions
{
    // Interface for the GitHub API.
    public interface IGitHubClient
    {
        Task<List<IssueModel>> GetIssues(string repository, DateTime lastRunDate);

        Task<List<CommitModel>> GetCommits(string repository, DateTime lastRunDate);

        Task<List<PullRequestModel>> GetPullRequests(string repository, DateTime lastRunDate);
    }
}
