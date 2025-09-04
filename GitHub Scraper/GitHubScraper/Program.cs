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
            ApplicationService _applicationService = new();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application");

            if (!_applicationService.Setup())
            {
                _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
                Environment.Exit(0);
            }

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Running Application");

            _applicationService.Run();

            Console.ReadLine();
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
