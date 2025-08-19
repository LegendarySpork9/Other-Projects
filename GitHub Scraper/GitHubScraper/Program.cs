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
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            Console.ReadLine();
        }
    }
}
