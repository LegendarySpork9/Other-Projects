using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Services;

namespace GitHubScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            LoggerService _loggerService = new();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Owner: {AppSettingsModel.Owner}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Repositories: {AppSettingsModel.Repositories.Length}");

            if (string.IsNullOrWhiteSpace(AppSettingsModel.BearerToken) || AppSettingsModel.BearerToken == StandardValues.MissingValues.BearerToken)
            {
                _loggerService.LogMessage(StandardValues.LoggerValues.Warning, "Valid authentication token not found. Please provide one in the app settings with the tag \"BearerToken\"");
                _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
                Environment.Exit(0);
            }

            else
            {
                _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {AppSettingsModel.BearerToken}");
            }

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            Console.ReadLine();
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
