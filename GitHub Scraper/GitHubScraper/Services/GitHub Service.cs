using GitHubScraper.Converters;
using GitHubScraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubScraper.Services
{
    internal class GitHubService
    {
        private LoggerService Logger = new();

        public List<IssueModel> GetIssues(string repository)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching issues from GitHub for {repository} repository");

            List<IssueModel> issues = [];

            try
            {
                string url = $"https://api.github.com/repos/{AppSettingsModel.Owner}/{repository}/issues?state=all&per_page=100";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {AppSettingsModel.BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    JArray responseContent = JArray.Parse(response.Content);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Issues Returned: {responseContent.Count}");

                    foreach (JObject issue in responseContent)
                    {
                        IssueModel? processedIssue = JsonConvert.DeserializeObject<IssueModel>(issue.ToString());

                        if (processedIssue != null)
                        {
                            processedIssue.Repository = repository;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched issue from GitHub for {repository} repository");
            return issues;
        }
    }
}
