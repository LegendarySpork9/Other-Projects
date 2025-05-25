using ServerSiteReporter.Converters;
using ServerSiteReporter.Models;
using ServerSiteReporter.Services;
using System.Configuration;

namespace ServerSiteReporter
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
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Base URL: {AppSettingsModel.BaseURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Credentials: {AppSettingsModel.Credentials}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Endpoints: {AppSettingsModel.Endpoints}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Payload Location: {AppSettingsModel.PayloadLocation}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Time: {AppSettingsModel.RefreshTime}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {AppSettingsModel.HostName}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Games: {ConfigurationManager.AppSettings["Games"]}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Components: {ConfigurationManager.AppSettings["Components"]}");

            _applicationService.Setup();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            _applicationService.Start();

            Console.ReadLine();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
