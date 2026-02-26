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
        private readonly IGitHubOptions _Options;
        private readonly ILoggerService _Logger;

        private readonly string BaseURL = "https://api.github.com";

        // Sets the class's global variables.
        public GitHubClientWrapper(
            IGitHubOptions _options,
            ILoggerService _logger)
        {
            _Options = _options;
            _Logger = _logger;
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

        private string BuildURL(string endpoint, string repository, DateTime? lastRunDate)
        {
            string url = $"{BaseURL}/repos/{_Options.Owner}/{repository}{endpoint}";
            string query = GitHubConverter.GetQuery(endpoint);

            if (lastRunDate.HasValue)
            {
                query += "&since={lastRunDate:yyyy-MM-ddTHH:mm:ssZ}";
            }

            url += query;

            return url;
        }
    }
}
