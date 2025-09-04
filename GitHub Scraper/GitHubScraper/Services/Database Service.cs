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

                    using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\Run History\GetLastRunDate.sql"), connection))
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
    }
}
