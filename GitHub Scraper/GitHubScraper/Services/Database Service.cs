// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using Microsoft.Data.SqlClient;

namespace GitHubScraper.Services
{
    public class DatabaseService : IDatabase
    {
        private readonly ILoggerService _Logger;
        private readonly IClock _Clock;
        private readonly IDatabaseOptions _Options;
        private readonly IFileSystem _FileSystem;

        // Sets the class's global variables.
        public DatabaseService(
            ILoggerService _logger,
            IClock _clock,
            IDatabaseOptions _options,
            IFileSystem _fileSystem)
        {
            _Logger = _logger;
            _Clock = _clock;
            _Options = _options;
            _FileSystem = _fileSystem;
        }

        // Gets the last time the application was run for the given repository.
        public DateTime GetLastRunDate(string repository)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining the last run date for repository {repository}");

            DateTime lastRunDate = _Clock.DefaultDate;

            try
            {
                using (SqlConnection connection = new(_Options.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(_FileSystem.ReadAllText($@"{_Options.SQLFiles}\GetLastRunDate.sql"), connection))
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                lastRunDate = dataReader.GetDateTime(0);
                                lastRunDate = DateTime.SpecifyKind(lastRunDate, DateTimeKind.Utc);
                            }

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Last Run Date: {lastRunDate:dd/MM/yyyy HH:mm:ss}");
                        }

                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained the last run date for repository {repository}");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to obtain the last run date for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            return lastRunDate;
        }

        // Gets the existing issues for the given repository.
        public List<IssueModel> GetIssues(string repository)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining the existing issues for repository {repository}");

            List<IssueModel> existingIssues = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\GetIssues.sql"), connection))
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                existingIssues.Add(new()
                                {
                                    Id = dataReader.GetInt64(0),
                                    Number = 0,
                                    Title = "UnLoaded",
                                    State = "UnLoaded",
                                    Created_At = dataReader.GetDateTime(1),
                                    Closed_At = dataReader.GetDateTime(2),
                                    Labels = []
                                });
                            }

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{existingIssues.Count} existsing issue(s)");
                        }

                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained the existing issues for repository {repository}");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to obtain the existing issues for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            return existingIssues;
        }

        // Outputs the issues to the database.
        public void OutputIssues(string repository, List<IssueModel> issues)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {issues.Count} issue(s) for repository {repository}");

            List<IssueModel> successful = [];
            List<IssueModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (IssueModel issue in issues)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting issue {issue.Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputIssue.sql"), connection))
                            {
                                int result = -1;

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", issue.Repository));
                                command.Parameters.Add(new SqlParameter("@issueId", issue.Id));
                                command.Parameters.Add(new SqlParameter("@number", issue.Number));
                                command.Parameters.Add(new SqlParameter("@title", issue.Title));
                                command.Parameters.Add(new SqlParameter("@assignee", issue.Assignee?.Login ?? "Unassigned"));
                                command.Parameters.Add(new SqlParameter("@type", issue.Type ?? "Undefined"));
                                command.Parameters.Add(new SqlParameter("@status", issue.State));
                                command.Parameters.Add(new SqlParameter("@dateCreated", issue.Created_At));
                                command.Parameters.Add(new SqlParameter("@dateSolved", issue.Closed_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        result = dataReader.GetInt32(0);
                                    }
                                }

                                if (result == 0)
                                {
                                    successful.Add(issue);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted issue {issue.Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(issue);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output issue {issue.Number}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the issue(s) for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (issues.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issues.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issues.Count) * 100}%) output errored");
            }
            
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {issues.Count} issue(s) for repository {repository}");
        }

        // Outputs the commits to the database.
        public void OutputCommits(string repository, List<CommitModel> commits)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {commits.Count} commit(s) for repository {repository}");

            List<CommitModel> successful = [];
            List<CommitModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (CommitModel commit in commits)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting commit {commit.Sha}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputCommit.sql"), connection))
                            {
                                int result = -1;

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", commit.Repository));
                                command.Parameters.Add(new SqlParameter("@author", commit.Commit.Author.Name));
                                command.Parameters.Add(new SqlParameter("@committer", commit.Commit.Committer.Name));
                                command.Parameters.Add(new SqlParameter("@sha", commit.Sha));
                                command.Parameters.Add(new SqlParameter("@message", commit.Commit.Message));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        result = dataReader.GetInt32(0);
                                    }
                                }

                                if (result == 0)
                                {
                                    successful.Add(commit);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted commit {commit.Sha}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(commit);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output commit {commit.Sha}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the commit(s) for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (commits.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / commits.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / commits.Count) * 100}%) output errored");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {commits.Count} commit(s) for repository {repository}");
        }

        // Outputs the pull requests to the database.
        public void OutputPullRequests(string repository, List<PullRequestModel> pullRequests)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {pullRequests.Count} pull request(s) for repository {repository}");

            List<PullRequestModel> successful = [];
            List<PullRequestModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (PullRequestModel pullRequest in pullRequests)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting pull request {pullRequest.Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputPullRequest.sql"), connection))
                            {
                                int result = -1;

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", pullRequest.Repository));
                                command.Parameters.Add(new SqlParameter("@pullRequestId", pullRequest.Id));
                                command.Parameters.Add(new SqlParameter("@number", pullRequest.Number));
                                command.Parameters.Add(new SqlParameter("@title", pullRequest.Title));
                                command.Parameters.Add(new SqlParameter("@assignee", pullRequest.Assignee?.Login ?? "Unassigned"));
                                command.Parameters.Add(new SqlParameter("@type", pullRequest.Type ?? "Undefined"));
                                command.Parameters.Add(new SqlParameter("@status", pullRequest.State));
                                command.Parameters.Add(new SqlParameter("@dateCreated", pullRequest.Created_At));
                                command.Parameters.Add(new SqlParameter("@dateSolved", pullRequest.Closed_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));
                                command.Parameters.Add(new SqlParameter("@dateMerged", pullRequest.Merged_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        result = dataReader.GetInt32(0);
                                    }
                                }

                                if (result == 0)
                                {
                                    successful.Add(pullRequest);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted pull request {pullRequest.Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(pullRequest);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output pull request {pullRequest.Number}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the pull request(s) for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (pullRequests.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / pullRequests.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / pullRequests.Count) * 100}%) output errored");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {pullRequests.Count} pull request(s) for repository {repository}");
        }

        // Outputs the workflow runs to the database.
        public void OutputWorkflowRuns(string repository, WorkflowModel workflow)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {workflow.WorkflowRuns.Count} workflow run(s) for {workflow.Name} workflow in repository {repository}");

            List<WorkflowRunModel> successful = [];
            List<WorkflowRunModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (WorkflowRunModel workflowRun in workflow.WorkflowRuns)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting workflow run {workflowRun.Run_Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputWorkflowRun.sql"), connection))
                            {
                                int result = -1;

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", workflowRun.RepositoryName));
                                command.Parameters.Add(new SqlParameter("@workflow", workflowRun.Name));
                                command.Parameters.Add(new SqlParameter("@workflowRunId", workflowRun.Id));
                                command.Parameters.Add(new SqlParameter("@runNumber", workflowRun.Run_Number));
                                command.Parameters.Add(new SqlParameter("@actor", workflowRun.Actor.Login));
                                command.Parameters.Add(new SqlParameter("@displayTitle", workflowRun.Display_Title));
                                command.Parameters.Add(new SqlParameter("@event", workflowRun.Event));
                                command.Parameters.Add(new SqlParameter("@status", workflowRun.Status));
                                command.Parameters.Add(new SqlParameter("@conclusion", workflowRun.Conclusion));
                                command.Parameters.Add(new SqlParameter("@dateCreated", workflowRun.Created_At));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        result = dataReader.GetInt32(0);
                                    }
                                }

                                if (result == 0)
                                {
                                    successful.Add(workflowRun);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted workflow run {workflowRun.Run_Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(workflowRun);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output workflow run {workflowRun.Run_Number}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the workflow run(s) for {workflow.Name} workflow in repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (workflow.WorkflowRuns.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / workflow.WorkflowRuns.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / workflow.WorkflowRuns.Count) * 100}%) output errored");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {workflow.WorkflowRuns.Count} workflow run(s) for {workflow.Name} workflow in repository {repository}");
        }

        // Outputs the releases to the database.
        public void OutputReleases(string repository, List<ReleaseModel> releases)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {releases.Count} release(s) for repository {repository}");

            List<ReleaseModel> successful = [];
            List<ReleaseModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (ReleaseModel release in releases)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting release {release.Id}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputRelease.sql"), connection))
                            {
                                int result = -1;

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", release.Repository));
                                command.Parameters.Add(new SqlParameter("@releaseId", release.Id));
                                command.Parameters.Add(new SqlParameter("@name", release.Name));
                                command.Parameters.Add(new SqlParameter("@author", release.Author.Login));
                                command.Parameters.Add(new SqlParameter("@draft", release.Draft));
                                command.Parameters.Add(new SqlParameter("@assets", release.Assets.Count));
                                command.Parameters.Add(new SqlParameter("@body", release.Body));
                                command.Parameters.Add(new SqlParameter("@dateCreated", release.Created_At));
                                command.Parameters.Add(new SqlParameter("@datePublished", release.Published_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        result = dataReader.GetInt32(0);
                                    }
                                }

                                if (result == 0)
                                {
                                    successful.Add(release);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted release {release.Id}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(release);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output release {release.Id}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the release(s) for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (releases.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / releases.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / releases.Count) * 100}%) output errored");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {releases.Count} release(s) for repository {repository}");
        }

        // Updates or inserts the issue aggregate record in the database.
        public void LogIssueAggregates(string repository, List<IssueAggregateModel> issueAggregates)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging {issueAggregates.Count} issue aggregate(s) for repository {repository}");

            List<IssueAggregateModel> successful = [];
            List<IssueAggregateModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (IssueAggregateModel issueAggregate in issueAggregates)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging issue aggregate for {issueAggregate.Date:dd/MM/yyyy HH:mm:ss}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\LogIssueAggregate.sql"), connection))
                            {
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", repository));
                                command.Parameters.Add(new SqlParameter("@date", issueAggregate.Date));
                                command.Parameters.Add(new SqlParameter("@created", issueAggregate.Created));
                                command.Parameters.Add(new SqlParameter("@solved", issueAggregate.Solved));

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected == 1)
                                {
                                    successful.Add(issueAggregate);
                                }

                                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged issue aggregate for {issueAggregate.Date:dd/MM/yyyy HH:mm:ss}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(issueAggregate);

                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log issue aggregate for {issueAggregate.Date:dd/MM/yyyy HH:mm:ss}. Error Message: {ex.Message}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log the issue aggregates(s) for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (issueAggregates.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issueAggregates.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issueAggregates.Count) * 100}%) output errored");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged {issueAggregates.Count} issue aggregate(s) for repository {repository}");
        }

        // Logs the run to the database.
        public void LogRun(string repository, int issues, int commits, int pullRequests, int workflowRuns, int releases)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging run for repository {repository}");

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\LogRun.sql"), connection))
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));
                        command.Parameters.Add(new SqlParameter("@issues", issues));
                        command.Parameters.Add(new SqlParameter("@commits", commits));
                        command.Parameters.Add(new SqlParameter("@pullRequests", pullRequests));
                        command.Parameters.Add(new SqlParameter("@workflowRuns", workflowRuns));
                        command.Parameters.Add(new SqlParameter("@releases", releases));

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 1)
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged run for repository {repository}");
                        }

                        else
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, $"An unknown error occured logging run for repository {repository}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"issues: {issues}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"commits: {commits}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"pullRequests: {pullRequests}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"workflowRuns: {workflowRuns}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"releases: {releases}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log run for repository {repository}. Error Message: {ex.Message}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }
        }
    }
}
