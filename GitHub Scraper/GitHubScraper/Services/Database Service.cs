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

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained the last run date for repository {repository}");
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to output the issues for repository {repository}. Error Message: {ex.Message}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"Full Error: {ex}");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{successful.Count} ({(successful.Count / issues.Count) * 100}%) output successful, {errored.Count} ({(errored.Count / issues.Count) * 100}%) output errored");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Outputted {issues.Count} issue(s) for repository {repository}");
        }
    }
}
