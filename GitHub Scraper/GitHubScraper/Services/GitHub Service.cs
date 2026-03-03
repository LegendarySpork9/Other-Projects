// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;

namespace GitHubScraper.Services
{
    public class GitHubService
    {
        private readonly ILoggerService _Logger;
        private readonly IGitHubClient _GitHubClient;

        // Sets the class's global variables.
        public GitHubService(
            ILoggerService _logger,
            IGitHubClient gitHubClient)
        {
            _Logger = _logger;
            _GitHubClient = gitHubClient;
        }

        // Returns a list of the issues for the repository.
        public List<IssueModel> GetIssues(string repository, DateTime lastRunDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching issues from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<IssueModel> issues = _GitHubClient.GetIssues(repository, lastRunDate).Result;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filtering out pull requests in favour of pull request endpoint data");

            int preFilterIssueCount = issues.Count;

            issues = [.. issues.Where(i => i.Pull_Request == null)];

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filtered out {preFilterIssueCount - issues.Count} pull request(s) in favour of pull request endpoint data");

            foreach (IssueModel issue in issues)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for issue {issue.Number}");

                issue.Repository = repository;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                foreach (LabelModel label in issue.Labels)
                {
                    if (GitHubConverter.IsType(label.Name))
                    {
                        issue.Type = GitHubConverter.GetType(label.Name);

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Type: {issue.Type}");

                        break;
                    }
                }

                issue.State = textInfo.ToTitleCase(issue.State);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {issue.State}");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for issue {issue.Number}");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Converting date times to UTC for issue {issue.Number}");

                issue.Created_At = issue.Created_At.UtcDateTime;

                if (issue.Closed_At.HasValue)
                {
                    issue.Closed_At = issue.Closed_At.Value.UtcDateTime;
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Converted date times to UTC for issue {issue.Number}");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {issues.Count} issue(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return [.. issues.OrderBy(i => i.Id)];
        }

        // Returns a list of the commits for the repository.
        public List<CommitModel> GetCommits(string repository, DateTime lastRunDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching commits from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<CommitModel> commits = _GitHubClient.GetCommits(repository, lastRunDate).Result;

            foreach (CommitModel commit in commits)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for commit {commit.Sha}");

                commit.Repository = repository;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for commit {commit.Sha}");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {commits.Count} commit(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return [.. commits.OrderBy(c => c.Commit.Committer.Date)];
        }

        // Returns a list of the pull requests for the repository.
        public List<PullRequestModel> GetPullRequests(string repository, DateTime lastRunDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching pull requests from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<PullRequestModel> pullRequests = _GitHubClient.GetPullRequests(repository, lastRunDate).Result;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            foreach (PullRequestModel pullRequest in pullRequests)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for pull request {pullRequest.Number}");

                pullRequest.Repository = repository;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                foreach (LabelModel label in pullRequest.Labels)
                {
                    if (GitHubConverter.IsType(label.Name))
                    {
                        pullRequest.Type = GitHubConverter.GetType(label.Name);

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Type: {pullRequest.Type}");

                        break;
                    }
                }

                pullRequest.State = textInfo.ToTitleCase(pullRequest.State);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"State: {pullRequest.State}");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for pull request {pullRequest.Number}");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Converting date times to UTC for issue {issue.Number}");

                pullRequest.Created_At = pullRequest.Created_At.UtcDateTime;
                pullRequest.Updated_At = pullRequest.Updated_At.UtcDateTime;

                if (pullRequest.Merged_At.HasValue)
                {
                    pullRequest.Merged_At = pullRequest.Merged_At.Value.UtcDateTime;
                }

                if (pullRequest.Closed_At.HasValue)
                {
                    pullRequest.Closed_At = pullRequest.Closed_At.Value.UtcDateTime;
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Converted date times to UTC for issue {issue.Number}");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {pullRequests.Count} pull request(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return [.. pullRequests.OrderBy(pr => pr.Id)];
        }

        // Returns a list of the workflow runs for the repository and workflow.
        public List<WorkflowRunModel>? GetWorkflowRuns(string repository, string workflow, DateTime lastRunDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching workflow runs from GitHub for {workflow} workflow in {repository} repository from {lastRunDate:dd/MM/yyyy 00:00:00}");

            List<WorkflowRunModel>? workflowRuns = [];
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            string url;
            int page = 1;

            try
            {
                if (lastRunDate == DateTime.Parse("1900-01-01 00:00:00").ToUniversalTime())
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/actions/workflows/{workflow}/runs?created=>1970-01-01T00:00:00Z&per_page=100";
                }

                else
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/actions/workflows/{workflow}/runs?created=>{DateTime.UtcNow:yyyy-MM-ddT00:00:00Z}&per_page=100";
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        JObject responseContent = JObject.Parse(response.Content);
                        JToken? workflowRunsToken = responseContent["workflow_runs"];

                        if (workflowRunsToken != null)
                        {
                            List<WorkflowRunModel> apiWorkflowRuns = JsonConvert.DeserializeObject<List<WorkflowRunModel>>(workflowRunsToken.ToString()) ?? [];

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Workflow Runs Returned: {apiWorkflowRuns.Count}");

                            if (apiWorkflowRuns.Count > 0)
                            {
                                foreach (WorkflowRunModel workflowRun in apiWorkflowRuns)
                                {
                                    if (workflowRun.Updated_At >= lastRunDate)
                                    {
                                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for workflow run {workflowRun.Run_Number}");

                                        workflowRun.RepositoryName = repository;

                                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                        workflowRun.Status = textInfo.ToTitleCase(workflowRun.Status);

                                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {workflowRun.Status}");

                                        workflowRun.Conclusion = textInfo.ToTitleCase(workflowRun.Conclusion);

                                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Conclusion: {workflowRun.Conclusion}");

                                        workflowRun.Event = textInfo.ToTitleCase(workflowRun.Event.Replace("_", " "));

                                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Event: {workflowRun.Event}");
                                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for workflow run {workflowRun.Run_Number}");

                                        workflowRuns.Add(workflowRun);
                                    }
                                }

                                page++;
                            }

                            else
                            {
                                break;
                            }
                        }

                        else
                        {
                            break;
                        }
                    }

                    else
                    {
                        if (workflowRuns.Count == 0)
                        {
                            workflowRuns = null;
                        }

                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            if (workflowRuns != null)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {workflowRuns.Count} workflow run(s) from GitHub for {workflow} workflow in {repository} repository from {lastRunDate:dd/MM/yyyy 00:00:00}");
                workflowRuns = [.. workflowRuns.OrderBy(wr => wr.Id)];
            }

            else
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched 0 workflow run(s) from GitHub for {workflow} workflow in {repository} repository from {lastRunDate:dd/MM/yyyy 00:00:00}");
            }

            return workflowRuns;
        }

        // Returns a list of the releases for the repository.
        public List<ReleaseModel> GetReleases(string repository, DateTime lastRunDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching releases from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<ReleaseModel> releases = [];

            int page = 1;

            try
            {
                string url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/releases?Per_Page=100";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<ReleaseModel> apiReleases = JsonConvert.DeserializeObject<List<ReleaseModel>>(response.Content) ?? [];

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Releases Returned: {apiReleases.Count}");

                        if (apiReleases.Count > 0)
                        {
                            foreach (ReleaseModel release in apiReleases)
                            {
                                if (release.Updated_At >= lastRunDate)
                                {
                                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for release {release.Id}");

                                    release.Repository = repository;

                                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                    release.NumberOfAssets = release.Assets.Count;

                                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for release {release.Id}");

                                    releases.Add(release);
                                }
                            }

                            page++;
                        }

                        else
                        {
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {releases.Count} release(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return [.. releases.OrderBy(r => r.Id)];
        }
    }
}
