using ServerStatusSite.Converters;
using ServerStatusSite.Models;
using System.Text;

namespace ServerStatusSite.Services
{
    public class DiscordService
    {
        private LoggerService Logger { get; set; }
        private AppSettingsModel AppSettings { get; set; }

        public DiscordService(LoggerService _loggerService, AppSettingsModel appSettings)
        {
            Logger = _loggerService;
            AppSettings = appSettings;
        }

        public bool SendNotification(string recipientId, string message)
        {
            if (AppSettings.SendAlerts)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Notification to Discord");

                bool successfulSend = false;

                try
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {AppSettings.WebookURL}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Recipient: {recipientId}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Message: {message}");

                    HttpClient client = new();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Client");

                    string payload = "{\"content\": \"<@&" + recipientId + "> " + message + "\"}";

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Payload: {payload}");

                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Content");

                    HttpRequestMessage request = new(HttpMethod.Post, AppSettings.WebookURL)
                    {
                        Content = content
                    };

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Request Message");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                    HttpResponseMessage response = client.Send(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        successfulSend = true;

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");
                    }
                }

                catch (Exception ex)
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                    Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sent Notification to Discord");
                return successfulSend;
            }

            return true;
        }
    }
}
