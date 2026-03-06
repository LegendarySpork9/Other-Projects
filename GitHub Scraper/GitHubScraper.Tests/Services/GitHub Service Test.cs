// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using GitHubScraper.Services;
using Moq;

namespace GitHubScraper.Tests.Services
{
    [TestClass]
    public class GitHubServiceTest
    {
        private DateTime Date;
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IClock> _MockClock = new();


        /// <summary>
        /// Sets the mocks up for the tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            Date = new(2026, 03, 05, 00, 00, 00, DateTimeKind.Utc);
        }

        /// <summary>
        /// Checks whether the GetIssues method returns a list of issues.
        /// </summary>
        [TestMethod]
        public void TestGetIssues()
        {
            List<IssueModel> mockIssue =
            [
                new()
                {
                    Repository = "Unit-Test",
                    Id = 46578346587688,
                    Number = 1,
                    Title = "Test",
                    Assignee = new()
                    {
                        Login = "UnitTester"
                    },
                    Type = "Bug",
                    State = "Closed",
                    Created_At = Date.AddDays(-1),
                    Closed_At = Date,
                    Labels =
                    [
                        new()
                        {
                            Name = "bug"
                        }
                    ]
                }
            ];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetIssues(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockIssue);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List <IssueModel> issues = _gitHubService.GetIssues("Unit-Test", _MockClock.Object.DefaultDate);

            Assert.IsTrue(issues.Count > 0);
            Assert.AreEqual(mockIssue[0].Id, issues[0].Id);
        }

        /// <summary>
        /// Checks whether the GetIssues method returns an empty list of issues.
        /// </summary>
        [TestMethod]
        public void TestGetIssuesEmpty()
        {
            List<IssueModel> mockIssue = [];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetIssues(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockIssue);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<IssueModel> issues = _gitHubService.GetIssues("Unit-Test", Date);

            Assert.IsTrue(issues.Count == 0);
        }

        /// <summary>
        /// Checks whether the GetCommits method returns a list of commits.
        /// </summary>
        [TestMethod]
        public void TestGetCommits()
        {
            List<CommitModel> mockCommit =
            [
                new()
                {
                    Repository = "Unit-Test",
                    Sha = "g5vyb65yg6ybhgbhvfgh665664637yvtvt",
                    Commit = new()
                    {
                        Author = new()
                        {
                            Name = "UnitTester"
                        },
                        Committer = new()
                        {
                            Name = "UnitTester"
                        },
                        Message = "This is a test message."
                    }
                }
            ];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetCommits(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockCommit);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<CommitModel> commits = _gitHubService.GetCommits("Unit-Test", _MockClock.Object.DefaultDate);

            Assert.IsTrue(commits.Count > 0);
            Assert.AreEqual(mockCommit[0].Sha, commits[0].Sha);
        }

        /// <summary>
        /// Checks whether the GetCommits method returns an empty list of commits.
        /// </summary>
        [TestMethod]
        public void TestGetCommitsEmpty()
        {
            List<CommitModel> mockCommit = [];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetCommits(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockCommit);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<CommitModel> commits = _gitHubService.GetCommits("Unit-Test", Date);

            Assert.IsTrue(commits.Count == 0);
        }

        /// <summary>
        /// Checks whether the GetPullRequests method returns a list of pull requests.
        /// </summary>
        [TestMethod]
        public void TestGetPullRequests()
        {
            List<PullRequestModel> mockPullRequest =
            [
                new()
                {
                    Repository = "Unit-Test",
                    Id = 46578346587688,
                    Number = 1,
                    Title = "Test",
                    Assignee = new()
                    {
                        Login = "UnitTester"
                    },
                    Type = "Bug",
                    State = "Closed",
                    Created_At = Date.AddDays(-1),
                    Updated_At = Date,
                    Closed_At = Date,
                    Merged_At = Date,
                    Labels =
                    [
                        new()
                        {
                            Name = "bug"
                        }
                    ]
                }
            ];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetPullRequests(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockPullRequest);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<PullRequestModel> pullRequests = _gitHubService.GetPullRequests("Unit-Test", _MockClock.Object.DefaultDate);

            Assert.IsTrue(pullRequests.Count > 0);
            Assert.AreEqual(mockPullRequest[0].Id, pullRequests[0].Id);
        }

        /// <summary>
        /// Checks whether the GetPullRequests method returns an empty list of pull requests.
        /// </summary>
        [TestMethod]
        public void TestGetPullRequestsEmpty()
        {
            List<PullRequestModel> mockPullRequest = [];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetPullRequests(It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockPullRequest);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<PullRequestModel> pullRequests = _gitHubService.GetPullRequests("Unit-Test", _MockClock.Object.DefaultDate);

            Assert.IsTrue(pullRequests.Count == 0);
        }

        /// <summary>
        /// Checks whether the GetWorkflowRuns method returns a list of workflow runs.
        /// </summary>
        [TestMethod]
        public void TestGetWorkflowRuns()
        {
            List<WorkflowRunModel> mockWorkflowRun =
            [
                new()
                {
                    Id = 46578346587688,
                    Run_Number = 1,
                    Actor = new()
                    {
                        Login = "UnitTester"
                    },
                    Name = "test_workflow",
                    Display_Title = "Test",
                    Event = "pull_request",
                    Status = "completed",
                    Conclusion = "success",
                    Created_At = Date,
                    Updated_At = Date
                }
            ];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetWorkflowRuns(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockWorkflowRun);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<WorkflowRunModel> workflowRuns = _gitHubService.GetWorkflowRuns("Unit-Test", "Test Workflow", _MockClock.Object.DefaultDate);

            Assert.IsTrue(workflowRuns.Count > 0);
            Assert.AreEqual(mockWorkflowRun[0].Id, workflowRuns[0].Id);
        }

        /// <summary>
        /// Checks whether the GetWorkflowRuns method returns an empty list of workflow runs.
        /// </summary>
        [TestMethod]
        public void TestGetWorkflowRunsEmpty()
        {
            List<WorkflowRunModel> mockWorkflowRun = [];

            Mock<IGitHubClient> _mockGitHubClient = new();
            _mockGitHubClient.Setup(ghc => ghc.GetWorkflowRuns(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result).Returns(mockWorkflowRun);

            GitHubService _gitHubService = new(_MockLogger.Object, _mockGitHubClient.Object);

            List<WorkflowRunModel> workflowRuns = _gitHubService.GetWorkflowRuns("Unit-Test", "Test Workflow", _MockClock.Object.DefaultDate);

            Assert.IsTrue(workflowRuns.Count == 0);
        }

        /*

        // Checks whether the GetReleases method returns a list of releases.
        [TestMethod]
        public void TestGetReleases()
        {
            Mock<GitHubService> _mockGitHubService = new();

            List<ReleaseModel> releases = _mockGitHubService.Object.GetReleases(AppSettingsModel.Repositories, DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime());

            Assert.IsTrue(releases.Count > 0);
        }*/
    }
}
