using Github_To_Codecks.Models;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Github_To_Codecks.Services
{
    internal class GithubService
    {
        GitTokenModel gitToken;

        public GithubService(GitTokenModel _gitToken)
        {
            gitToken = _gitToken;
        }

        // Gets all the repos I own.
        public async Task<List<RepoModel>> GetRepos()
        {
            List<RepoModel> repos = new List<RepoModel>();

            RestClient rest = new RestClient(@"https://api.github.com/user/repos");
            RestRequest request = new RestRequest
            {
                Method = Method.Get
            };
            request.AddHeader("Authorization", $"Bearer {gitToken.Token}");
            request.AddHeader("Accept", "application/vnd.github+json");
            request.AddHeader("X-GitHub-Api-Version", "2022-11-28");
            request.AddParameter("affiliation", "owner");
            RestResponse response = await rest.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JArray repoArray = JArray.Parse(response.Content);

                foreach (JObject item in repoArray)
                {
                    RepoModel repo = new RepoModel
                    {
                        Name = item["name"].ToString(),
                        FormattedName = item["name"].ToString().Replace("-", " ")
                    };

                    repos.Add(repo);
                }
            }

            return repos;
        }

        // Gets all the open issues in the repo.
        public async Task<List<IssueModel>> GetIssues(string repoName)
        {
            List<IssueModel> issues = new List<IssueModel>();
            string url = $"https://api.github.com/repos/LegendarySpork9/{repoName}/issues";

            RestClient rest = new RestClient(url);
            RestRequest request = new RestRequest
            {
                Method = Method.Get
            };
            request.AddHeader("Authorization", $"Bearer {gitToken.Token}");
            request.AddHeader("Accept", "application/vnd.github+json");
            request.AddHeader("X-GitHub-Api-Version", "2022-11-28");
            request.AddParameter("per_page", "100");
            RestResponse response = await rest.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JArray issueArray = JArray.Parse(response.Content);

                foreach (JObject item in issueArray)
                {
                    List<string> tags = new List<string>();
                    JArray tagArray = JArray.Parse(item["labels"].ToString());

                    foreach (JObject tag in tagArray)
                    {
                        tags.Add(tag["name"].ToString());
                    }

                    IssueModel issue = new IssueModel
                    {
                        Number = int.Parse(item["number"].ToString()),
                        Title = item["title"].ToString(),
                        Body = item["body"].ToString(),
                        Tags = tags
                    };

                    issues.Add(issue);
                }
            }

            return issues;
        }
    }
}
