using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;

namespace GitHubScraper.Services
{
    internal class GitHubService
    {
        private readonly LoggerService Logger = new();

        // Returns a lits of the issues for the repository.
        public List<IssueModel> GetIssues(string repository, DateTime lastRunDate)
        {
            GitHubConverter _gitHubConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching issues from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<IssueModel> issues = [];

            string url;
            int page = 1;

            try
            {
                if (lastRunDate == DateTime.Parse("1900-01-01 00:00:00"))
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/issues?state=all&sort=updated&per_page=100";
                }

                else
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/issues?state=all&sort=updated&since={lastRunDate:yyyy-MM-ddTHH:mm:ssZ}&per_page=100";
                }

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<IssueModel> apiIssues = JsonConvert.DeserializeObject<List<IssueModel>>(response.Content) ?? [];

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Issues Returned: {issues.Count}");

                        if (apiIssues.Count > 0)
                        {
                            foreach (IssueModel issue in apiIssues)
                            {
                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for issue {issue.Number}");

                                issue.Repository = repository;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                foreach (LabelModel label in issue.Labels)
                                {
                                    if (_gitHubConverter.IsType(label.Name))
                                    {
                                        issue.Type = _gitHubConverter.GetType(label.Name);

                                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Type: {issue.Type}");

                                        break;
                                    }
                                }

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for issue {issue.Number}");

                                issues.Add(issue);
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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {issues.Count} issue(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return issues;
        }

        // Returns a list of the commits for the repository.
        public List<CommitModel> GetCommits(string repository, DateTime lastRunDate)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching commits from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<CommitModel> commits = [];

            string url;
            int page = 1;

            try
            {
                if (lastRunDate == DateTime.Parse("1900-01-01 00:00:00"))
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/commits?per_page=100";
                }

                else
                {
                    url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/commits?since={lastRunDate:yyyy-MM-ddTHH:mm:ssZ}&per_page=100";
                }

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<CommitModel> apiCommits = JsonConvert.DeserializeObject<List<CommitModel>>(response.Content) ?? [];

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Commits Returned: {commits.Count}");

                        if (apiCommits.Count > 0)
                        {
                            foreach (CommitModel commit in apiCommits)
                            {
                                Logger.LogMessage(StandardValues.LoggerValues.Info, "Filling blanks for commit");

                                commit.Repository = repository;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");
                                Logger.LogMessage(StandardValues.LoggerValues.Info, "Filled blanks for commit");

                                commits.Add(commit);
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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {commits} commit(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return commits;
        }

        // Returns a list of the pull requests for the repository.
        public List<PullRequestModel> GetPullRequests(string repository, DateTime lastRunDate)
        {
            GitHubConverter _gitHubConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching pull requests from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<PullRequestModel> pullRequests = [];

            int page = 1;

            try
            {
                string url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/pulls?state=all&sort=updated&direction=desc&per_page=100";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<PullRequestModel> apiPullRequests = JsonConvert.DeserializeObject<List<PullRequestModel>>(response.Content) ?? [];

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Pull Requests Returned: {pullRequests.Count}");

                        if (apiPullRequests.Count > 0)
                        {
                            bool nextPage = true;

                            foreach (PullRequestModel pullRequest in apiPullRequests)
                            {
                                if (pullRequest.Updated_At >= lastRunDate)
                                {
                                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for pull request {pullRequest.Number}");

                                    pullRequest.Repository = repository;

                                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                    foreach (LabelModel label in pullRequest.Labels)
                                    {
                                        if (_gitHubConverter.IsType(label.Name))
                                        {
                                            pullRequest.Type = _gitHubConverter.GetType(label.Name);

                                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Type: {pullRequest.Type}");

                                            break;
                                        }
                                    }

                                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for pull request {pullRequest.Number}");

                                    pullRequests.Add(pullRequest);
                                }

                                else
                                {
                                    nextPage = false;
                                    break;
                                }
                            }

                            if (nextPage)
                            {
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
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {pullRequests} pull request(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return pullRequests;
        }

        // Returns a list of the workflow runs for the repository and workflow.
        public List<WorkflowRunModel>? GetWorkflowRuns(string repository, string workflow, DateTime lastRunDate)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching workflow runs from GitHub for {workflow} workflow in {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<WorkflowRunModel>? workflowRuns = [];
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            int page = 1;

            try
            {
                string url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/actions/workflows/{workflow}/runs?created=>{DateTime.UtcNow:yyyy-MM-ddT00:00:00Z}&per_page=100";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        JObject responseContent = JObject.Parse(response.Content);
                        JToken? workflowRunsToken = responseContent["workflow_runs"];

                        if (workflowRunsToken != null)
                        {
                            List<WorkflowRunModel> apiWorkflowRuns = JsonConvert.DeserializeObject<List<WorkflowRunModel>>(workflowRunsToken.ToString()) ?? [];

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Workflow Runs Returned: {apiWorkflowRuns.Count}");

                            if (apiWorkflowRuns.Count > 0)
                            {
                                foreach (WorkflowRunModel workflowRun in apiWorkflowRuns)
                                {
                                    if (workflowRun.Updated_At >= lastRunDate)
                                    {
                                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for workflow run {workflowRun.Run_Number}");

                                        workflowRun.RepositoryName = repository;

                                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                        workflowRun.Status = textInfo.ToTitleCase(workflowRun.Status);

                                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {workflowRun.Status}");

                                        workflowRun.Conclusion = textInfo.ToTitleCase(workflowRun.Conclusion);

                                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Conclusion: {workflowRun.Conclusion}");
                                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for workflow run {workflowRun.Run_Number}");

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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {workflowRuns} workflow run(s) from GitHub for {workflow} workflow in {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return workflowRuns;
        }

        // Returns a list of the releases for the repository.
        public List<ReleaseModel> GetReleases(string repository, DateTime lastRunDate)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching releases from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");

            List<ReleaseModel> releases = [];

            int page = 1;

            try
            {
                string url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/releases?Per_Page=100";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                while (true)
                {
                    RestRequest request = new()
                    {
                        Method = Method.Get
                    };
                    request.AddParameter("page", page);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page: {page}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    RestResponse response = client.Execute(request);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<ReleaseModel> apiReleases = JsonConvert.DeserializeObject<List<ReleaseModel>>(response.Content) ?? [];

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Releases Returned: {apiReleases.Count}");

                        if (apiReleases.Count > 0)
                        {
                            foreach (ReleaseModel release in apiReleases)
                            {
                                if (release.Updated_At >= lastRunDate)
                                {
                                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filling blanks for release {release.Id}");

                                    release.Repository = repository;

                                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");

                                    release.NumberOfAssets = release.Assets.Count;

                                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Filled blanks for release {release.Id}");

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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched {releases} release(s) from GitHub for {repository} repository from {lastRunDate:dd/MM/yyyy HH:mm:ss}");
            return releases;
        }
    }
}
