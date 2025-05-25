using System.Configuration;

namespace ServerSiteAutomation.Models
{
    public static class AppSettingsModel
    {
        public static string WebookURL { get; set; } = ConfigurationManager.AppSettings["WebhookURL"];
        public static string RecipientId { get; set; } = ConfigurationManager.AppSettings["RecipientId"];
        public static string BaseURL { get; set; } = ConfigurationManager.AppSettings["APIBaseURL"];
        public static string Credentials { get; set; } = ConfigurationManager.AppSettings["APICredentials"];
        public static string Endpoints { get; set; } = ConfigurationManager.AppSettings["APIEndpoints"];
        public static string PayloadLocation { get; set; } = ConfigurationManager.AppSettings["APIPayloadLocation"];
        public static int RefreshTime { get; set; } = int.Parse(ConfigurationManager.AppSettings["RefreshTime"]);
    }
}
