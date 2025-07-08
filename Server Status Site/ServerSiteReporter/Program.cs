using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Services;
using ServerSiteReporter.Models;
using ServerSiteReporter.Services;
using System.Configuration;
using System.Reflection;

namespace ServerSiteReporter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            LoggerService _loggerService = new();
            _loggerService.ChangeIdentifier("Reporter");
            SharedSettingsModel sharedSettings = SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig($"{Assembly.GetExecutingAssembly().Location}.config"));

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Base URL: {sharedSettings.BaseURL}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Credentials: {sharedSettings.Credentials}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Endpoints: {sharedSettings.Endpoints}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"API Payload Location: {sharedSettings.PayloadLocation}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Time: {sharedSettings.RefreshTime}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {AppSettingsModel.HostName}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Games: {ConfigurationManager.AppSettings["Games"]}");
            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, $"Components: {ConfigurationManager.AppSettings["Components"]}");

            ApplicationService _applicationService = new(sharedSettings);
            _applicationService.SetLogger(_loggerService);
            _applicationService.Setup();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Configured Application");

            _applicationService.Start();

            Console.ReadLine();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
