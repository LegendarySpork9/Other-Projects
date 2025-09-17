using GitHubScraper.Converters;
using GitHubScraper.Models;
using Microsoft.Data.SqlClient;

namespace GitHubScraper.Services
{
    internal class DatabaseService
    {
        private readonly LoggerService Logger = new();

        // Gets the last time the application was run for the given repository.
        public DateTime GetLastRunDate(string repository)
        {
            DateTime lastRunDate = DateTime.Parse("01/01/1900");

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining the last run date for repository {repository}");

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\GetLastRunDate.sql"), connection))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Parameters Set");

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                lastRunDate = dataReader.GetDateTime(0);
                            }

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Last Run Date: {lastRunDate}");
                        }
                    }
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained the last run date for repository {repository}");
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to obtain the last run date for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            return lastRunDate;
        }

        // Outputs the issues to the database.
        public void OutputIssues(string repository, List<IssueModel> issues)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {issues.Count} issue(s) for repository {repository}");

            List<IssueModel> successful = [];
            List<IssueModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (IssueModel issue in issues)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting issue {issue.Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputIssue.sql"), connection))
                            {
                                int result = -1;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", issue.Repository));
                                command.Parameters.Add(new SqlParameter("@issueId", issue.Id));
                                command.Parameters.Add(new SqlParameter("@number", issue.Number));
                                command.Parameters.Add(new SqlParameter("@title", issue.Title));
                                command.Parameters.Add(new SqlParameter("@assignee", issue.Assignee?.Login ?? "Unassigned"));
                                command.Parameters.Add(new SqlParameter("@type", issue.Type));
                                command.Parameters.Add(new SqlParameter("@status", issue.State));
                                command.Parameters.Add(new SqlParameter("@dateCreated", issue.Created_At));
                                command.Parameters.Add(new SqlParameter("@dateSolved", issue.Closed_At));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Parameters Set");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted issue {issue.Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(issue);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output issue {issue.Number}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the issue(s) for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issues.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issues.Count) * 100}%) output errored");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {issues.Count} issue(s) for repository {repository}");
        }

        // Outputs the commits to the database.
        public void OutputCommits(string repository, List<CommitModel> commits)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {commits.Count} commit(s) for repository {repository}");

            List<CommitModel> successful = [];
            List<CommitModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (CommitModel commit in commits)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting commit {commit.Sha}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputCommit.sql"), connection))
                            {
                                int result = -1;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", commit.Repository));
                                command.Parameters.Add(new SqlParameter("@author", commit.Commit.Author.Name));
                                command.Parameters.Add(new SqlParameter("@committer", commit.Commit.Committer.Name));
                                command.Parameters.Add(new SqlParameter("@sha", commit.Sha));
                                command.Parameters.Add(new SqlParameter("@message", commit.Commit.Message));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Parameters Set");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted commit {commit.Sha}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(commit);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output commit {commit.Sha}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the commit(s) for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / commits.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / commits.Count) * 100}%) output errored");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {commits.Count} commit(s) for repository {repository}");
        }

        // Outputs the pull requests to the database.
        public void OutputPullRequests(string repository, List<PullRequestModel> pullRequests)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {pullRequests.Count} pull request(s) for repository {repository}");

            List<PullRequestModel> successful = [];
            List<PullRequestModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (PullRequestModel pullRequest in pullRequests)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting pull request {pullRequest.Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputPullRequest.sql"), connection))
                            {
                                int result = -1;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", pullRequest.Repository));
                                command.Parameters.Add(new SqlParameter("@pullRequestId", pullRequest.Id));
                                command.Parameters.Add(new SqlParameter("@number", pullRequest.Number));
                                command.Parameters.Add(new SqlParameter("@title", pullRequest.Title));
                                command.Parameters.Add(new SqlParameter("@assignee", pullRequest.Assignee?.Login ?? "Unassigned"));
                                command.Parameters.Add(new SqlParameter("@type", pullRequest.Type));
                                command.Parameters.Add(new SqlParameter("@status", pullRequest.State));
                                command.Parameters.Add(new SqlParameter("@dateCreated", pullRequest.Created_At));
                                command.Parameters.Add(new SqlParameter("@dateSolved", pullRequest.Closed_At));
                                command.Parameters.Add(new SqlParameter("@dateMerged", pullRequest.Merged_At));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Parameters Set");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted pull request {pullRequest.Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(pullRequest);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output pull request {pullRequest.Number}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the pull request(s) for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / pullRequests.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / pullRequests.Count) * 100}%) output errored");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {pullRequests.Count} pull request(s) for repository {repository}");
        }
    }
}
