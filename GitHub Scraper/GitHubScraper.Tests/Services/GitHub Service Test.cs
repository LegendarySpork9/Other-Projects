using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using GitHubScraper.Services;
using Moq;
using System.Configuration;

namespace GitHubScraper.Tests.Services
{
    [TestClass]
    public class GitHubServiceTest
    {
        // Sets the AppSettingsModel values to those in the test config file.
        [TestInitialize]
        public void Setup()
        {
            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
        }

        // Checks whether the GetIssues method returns a list of issues.
        [TestMethod]
        public void TestGetIssues()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<IssueModel> issues = _mockGitHubService.Object.GetIssues(AppSettingsModel.Repositories, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsTrue(issues.Count > 0);
        }

        // Checks whether the GetCommits method returns a list of commits.
        [TestMethod]
        public void TestGetCommits()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<CommitModel> commits = _mockGitHubService.Object.GetCommits(AppSettingsModel.Repositories, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsTrue(commits.Count > 0);
        }

        // Checks whether the GetPullRequests method returns a list of pull requests.
        [TestMethod]
        public void TestGetPullRequests()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<PullRequestModel> pullRequests = _mockGitHubService.Object.GetPullRequests(AppSettingsModel.Repositories, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsTrue(pullRequests.Count > 0);
        }

        // Checks whether the GetWorkflowRuns method returns a list of workflow runs.
        [TestMethod]
        public void TestGetWorkflowRuns()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<WorkflowRunModel> workflowRuns = _mockGitHubService.Object.GetWorkflowRuns(AppSettingsModel.Repositories, AppSettingsModel.Workflows, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsNotNull(workflowRuns);
            Assert.IsTrue(workflowRuns.Count > 0);
        }

        // Checks whether the GetReleases method returns a list of releases.
        [TestMethod]
        public void TestGetReleases()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<ReleaseModel> releases = _mockGitHubService.Object.GetReleases(AppSettingsModel.Repositories, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsTrue(releases.Count > 0);
        }
    }
}
