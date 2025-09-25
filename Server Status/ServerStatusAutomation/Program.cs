using ServerSiteAutomation.Services;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Services;
using System.Reflection;

namespace ServerSiteAutomation
{
    internal class Program
    {
        // Configures the application at startup.
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            LoggerService _loggerService = new();
            _loggerService.ChangeIdentifier("Automation");
            SharedSettingsModel sharedSettings = SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig($"{Assembly.GetExecutingAssembly().Location}.config"));

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Webhook URL: {sharedSettings.WebhookURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Recipient Id: {sharedSettings.RecipientId}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Base URL: {sharedSettings.BaseURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Credentials: {sharedSettings.Credentials}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Endpoints: {sharedSettings.Endpoints}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Payload Location: {sharedSettings.PayloadLocation}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Time: {sharedSettings.RefreshTime}");

            AutomationService _automationService = new(sharedSettings);
            _automationService.SetLogger(_loggerService);
            _automationService.Setup();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            _automationService.Start();

            Console.ReadLine();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
