// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Converters;
using GitHubScraper.Models;
using Newtonsoft.Json;
using RestSharp;

namespace GitHubScraper.Implementations
{
    public class GitHubClientWrapper : IGitHubClient
    {
        private readonly ILoggerService _Logger;
        private readonly IGitHubOptions _Options;

        private readonly string BaseURL = "https://api.github.com";

        // Sets the class's global variables.
        public GitHubClientWrapper(
            ILoggerService _logger,
            IGitHubOptions _options)
        {
            _Logger = _logger;
            _Options = _options;
        }

        // Returns a list of the issues for the repository.
        public async Task<List<IssueModel>> GetIssues(string repository, DateTime lastRunDate)
        {
            List<IssueModel> issues = [];
            int page = 1;

            try
            {
                string url = BuildURL("/issues", repository, lastRunDate);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {_Options.BearerToken}");

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

                    RestResponse response = await client.ExecuteAsync(request);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<IssueModel> apiIssues = JsonConvert.DeserializeObject<List<IssueModel>>(response.Content) ?? [];

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Issues Returned: {apiIssues.Count}");

                        if (apiIssues.Count > 0)
                        {
                            issues.AddRange(apiIssues);
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

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return issues;
        }

        // Returns a list of the commits for the repository.
        public async Task<List<CommitModel>> GetCommits(string repository, DateTime lastRunDate)
        {
            List<CommitModel> commits = [];
            int page = 1;

            try
            {
                string url = BuildURL("/commits", repository, lastRunDate);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {_Options.BearerToken}");

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

                    RestResponse response = await client.ExecuteAsync(request);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<CommitModel> apiCommits = JsonConvert.DeserializeObject<List<CommitModel>>(response.Content) ?? [];

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Issues Returned: {apiCommits.Count}");

                        if (apiCommits.Count > 0)
                        {
                            commits.AddRange(apiCommits);
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

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return commits;
        }

        // Returns a list of the pull requests for the repository.
        public async Task<List<PullRequestModel>> GetPullRequests(string repository, DateTime lastRunDate)
        {
            List<PullRequestModel> pullRequests = [];
            int page = 1;

            try
            {
                string url = BuildURL("/pulls", repository, null);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {_Options.BearerToken}");

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

                    RestResponse response = await client.ExecuteAsync(request);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                    {
                        List<PullRequestModel> apiPullRequests = JsonConvert.DeserializeObject<List<PullRequestModel>>(response.Content) ?? [];

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Issues Returned: {apiPullRequests.Count}");

                        if (apiPullRequests.Count > 0)
                        {
                            int pullRequestsToIgnore = apiPullRequests.Where(apr => apr.Updated_At < lastRunDate).ToList().Count;

                            if (pullRequestsToIgnore > 0)
                            {
                                pullRequests.AddRange(apiPullRequests.Where(apr => apr.Updated_At >= lastRunDate));
                                break;
                            }

                            else
                            {
                                pullRequests.AddRange(apiPullRequests);
                                page++;
                            }
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

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pullRequests;
        }

        private string BuildURL(string endpoint, string repository, DateTime? lastRunDate)
        {
            string url = $"{BaseURL}/repos/{_Options.Owner}/{repository}{endpoint}";
            string query = GitHubConverter.GetQuery(endpoint);

            if (lastRunDate.HasValue)
            {
                query += $"&since={lastRunDate:yyyy-MM-ddTHH:mm:ssZ}";
            }

            url += query;

            return url;
        }
    }
}
