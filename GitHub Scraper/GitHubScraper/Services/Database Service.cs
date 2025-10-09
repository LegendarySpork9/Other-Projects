using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;
using Microsoft.Data.SqlClient;

namespace GitHubScraper.Services
{
    internal class DatabaseService
    {
        private readonly LoggerService Logger = new();

        // Gets the last time the application was run for the given repository.
        public DateTime GetLastRunDate(string repository)
        {
            DateTime lastRunDate = DateTime.Parse("01/01/1900").ToUniversalTime();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining the last run date for repository {repository}");

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\GetLastRunDate.sql"), connection))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                lastRunDate = dataReader.GetDateTime(0).ToUniversalTime();
                            }

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Last Run Date: {lastRunDate}");
                        }

                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained the last run date for repository {repository}");
                    }
                }
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

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", issue.Repository));
                                command.Parameters.Add(new SqlParameter("@issueId", issue.Id));
                                command.Parameters.Add(new SqlParameter("@number", issue.Number));
                                command.Parameters.Add(new SqlParameter("@title", issue.Title));
                                command.Parameters.Add(new SqlParameter("@assignee", issue.Assignee?.Login ?? "Unassigned"));
                                command.Parameters.Add(new SqlParameter("@type", issue.Type ?? "Undefined"));
                                command.Parameters.Add(new SqlParameter("@status", issue.State));
                                command.Parameters.Add(new SqlParameter("@dateCreated", issue.Created_At));
                                command.Parameters.Add(new SqlParameter("@dateSolved", issue.Closed_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

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

            if (issues.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issues.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issues.Count) * 100}%) output errored");
            }
            
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

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", commit.Repository));
                                command.Parameters.Add(new SqlParameter("@author", commit.Commit.Author.Name));
                                command.Parameters.Add(new SqlParameter("@committer", commit.Commit.Committer.Name));
                                command.Parameters.Add(new SqlParameter("@sha", commit.Sha));
                                command.Parameters.Add(new SqlParameter("@message", commit.Commit.Message));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

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

            if (commits.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / commits.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / commits.Count) * 100}%) output errored");
            }

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

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

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

            if (pullRequests.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / pullRequests.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / pullRequests.Count) * 100}%) output errored");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {pullRequests.Count} pull request(s) for repository {repository}");
        }

        // Outputs the workflow runs to the database.
        public void OutputWorkflowRuns(string repository, WorkflowModel workflow)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {workflow.WorkflowRuns.Count} workflow run(s) for {workflow.Name} workflow in repository {repository}");

            List<WorkflowRunModel> successful = [];
            List<WorkflowRunModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (WorkflowRunModel workflowRun in workflow.WorkflowRuns)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting workflow run {workflowRun.Run_Number}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputWorkflowRun.sql"), connection))
                            {
                                int result = -1;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted workflow run {workflowRun.Run_Number}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(workflowRun);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output workflow run {workflowRun.Run_Number}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the workflow run(s) for {workflow.Name} workflow in repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (workflow.WorkflowRuns.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / workflow.WorkflowRuns.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / workflow.WorkflowRuns.Count) * 100}%) output errored");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {workflow.WorkflowRuns.Count} workflow run(s) for {workflow.Name} workflow in repository {repository}");
        }

        // Outputs the releases to the database.
        public void OutputReleases(string repository, List<ReleaseModel> releases)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting {releases.Count} release(s) for repository {repository}");

            List<ReleaseModel> successful = [];
            List<ReleaseModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (ReleaseModel release in releases)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputting release {release.Id}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputRelease.sql"), connection))
                            {
                                int result = -1;

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", release.Repository));
                                command.Parameters.Add(new SqlParameter("@releaseId", release.Id));
                                command.Parameters.Add(new SqlParameter("@name", release.Name));
                                command.Parameters.Add(new SqlParameter("@author", release.Author.Login));
                                command.Parameters.Add(new SqlParameter("@draft", release.Draft));
                                command.Parameters.Add(new SqlParameter("@assets", release.Assets.Count));
                                command.Parameters.Add(new SqlParameter("@body", release.Body));
                                command.Parameters.Add(new SqlParameter("@dateCreated", release.Created_At));
                                command.Parameters.Add(new SqlParameter("@datePublished", release.Published_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

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

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted release {release.Id}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(release);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output release {release.Id}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the release(s) for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (releases.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / releases.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / releases.Count) * 100}%) output errored");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {releases.Count} release(s) for repository {repository}");
        }

        // Updates or inserts the issue aggregate record in the database.
        public void LogIssueAggregates(string repository, List<IssueAggregateModel> issueAggregates)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging {issueAggregates.Count} issue aggregate(s) for repository {repository}");

            List<IssueAggregateModel> successful = [];
            List<IssueAggregateModel> errored = [];

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    foreach (IssueAggregateModel issueAggregate in issueAggregates)
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging issue aggregate for {issueAggregate.Date}");

                        try
                        {
                            using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\LogIssueAggregate.sql"), connection))
                            {
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                                command.Parameters.Add(new SqlParameter("@repository", repository));
                                command.Parameters.Add(new SqlParameter("@date", issueAggregate.Date));
                                command.Parameters.Add(new SqlParameter("@created", issueAggregate.Created));
                                command.Parameters.Add(new SqlParameter("@solved", issueAggregate.Solved));

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected == 1)
                                {
                                    successful.Add(issueAggregate);
                                }

                                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged issue aggregate for {issueAggregate.Date}");
                            }
                        }

                        catch (Exception ex)
                        {
                            errored.Add(issueAggregate);

                            Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log issue aggregate for {issueAggregate.Date}. Error Message: {ex.Message}");
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log the issue aggregates(s) for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            if (issueAggregates.Count > 0)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issueAggregates.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issueAggregates.Count) * 100}%) output errored");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged {issueAggregates.Count} issue aggregate(s) for repository {repository}");
        }

        // Logs the run to the database.
        public void LogRun(string repository, int issues, int commits, int pullRequests, int workflowRuns, int releases)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logging run for repository {repository}");

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "SQL Connection Opened");

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\LogRun.sql"), connection))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Command Loaded");

                        command.Parameters.Add(new SqlParameter("@repository", repository));
                        command.Parameters.Add(new SqlParameter("@issues", issues));
                        command.Parameters.Add(new SqlParameter("@commits", commits));
                        command.Parameters.Add(new SqlParameter("@pullRequests", pullRequests));
                        command.Parameters.Add(new SqlParameter("@workflowRuns", workflowRuns));
                        command.Parameters.Add(new SqlParameter("@releases", releases));

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Parameters Set");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Executing Query");

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 1)
                        {
                            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Logged run for repository {repository}");
                        }

                        else
                        {
                            Logger.LogMessage(StandardValues.LoggerValues.Error, $"An unknown error occured logging run for repository {repository}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repository: {repository}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"issues: {issues}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"commits: {commits}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"pullRequests: {pullRequests}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"workflowRuns: {workflowRuns}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"releases: {releases}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to log run for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }
        }
    }
}
