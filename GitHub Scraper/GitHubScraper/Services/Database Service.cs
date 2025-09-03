using GitHubScraper.Converters;
using GitHubScraper.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubScraper.Services
{
    internal class DatabaseService
    {
        LoggerService Logger = new();

        public DateTime GetLastRunDate(string repository)
        {
            DateTime lastRunDate = DateTime.Parse("01/01/1900");

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining the last run date for repository {repository}");

            try
            {
                using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new(File.ReadAllText()))
                }
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
