using ServerSiteAutomation.Converters;
using ServerSiteAutomation.Models;
using System.Text;

namespace ServerSiteAutomation.Services
{
    public class DiscordService
    {
        private readonly LoggerService Logger = new();

        public bool SendNotification(string message)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Notification to Discord");

            bool successfulSend = false;

            try
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {AppSettingsModel.WebookURL}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Recipient: {AppSettingsModel.RecipientId}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Message: {message}");

                HttpClient client = new();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Client");

                string payload = "{\"content\": \"<@&" + AppSettingsModel.RecipientId + "> " + message + "\"}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Payload: {payload}");

                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Http Content");

                HttpRequestMessage request = new(HttpMethod.Post, AppSettingsModel.WebookURL)
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
    }
}
