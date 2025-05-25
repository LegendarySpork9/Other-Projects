using ServerSiteAutomation.Converters;
using ServerSiteAutomation.Models;
using ServerSiteAutomation.Services;

namespace ServerSiteAutomation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            LoggerService _loggerService = new();
            AutomationService _automationService = new();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Webhook URL: {AppSettingsModel.WebookURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Recipient Id: {AppSettingsModel.RecipientId}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Base URL: {AppSettingsModel.BaseURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Credentials: {AppSettingsModel.Credentials}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Endpoints: {AppSettingsModel.Endpoints}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Payload Location: {AppSettingsModel.PayloadLocation}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Time: {AppSettingsModel.RefreshTime}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            _automationService.Setup();
            _automationService.Start();

            Console.ReadLine();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
