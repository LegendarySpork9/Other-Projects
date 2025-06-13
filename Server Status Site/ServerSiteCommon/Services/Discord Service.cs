using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using System.Text;

namespace ServerSiteCommon.Services
{
    public class DiscordService
    {
        private LoggerService Logger { get; set; }
        private SharedSettingsModel SharedSettings { get; set; }

        // Sets the class's global variables.
        public DiscordService(SharedSettingsModel sharedSettings)
        {
            SharedSettings = sharedSettings;
        }

        // Sets the logger.
        public void SetLogger(LoggerService _loggerService)
        {
            Logger = _loggerService;
        }

        // Sends a message to the given webhook URL.
        public bool SendNotification(string recipientId, string message)
        {
            if (SharedSettings.SendAlerts)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Notification to Discord");

                bool successfulSend = false;

                try
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {SharedSettings.WebhookURL}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Recipient: {recipientId}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Message: {message}");

                    HttpClient client = new();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Client");

                    string payload = "{\"content\": \"<@&" + recipientId + "> " + message + "\"}";

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Payload: {payload}");

                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Content");

                    HttpRequestMessage request = new(HttpMethod.Post, SharedSettings.WebhookURL)
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
