using System.Configuration;

namespace ServerSiteReporter.Models
{
    public static class AppSettingsModel
    {
        public static string BaseURL { get; set; } = ConfigurationManager.AppSettings["APIBaseURL"];
        public static string Credentials { get; set; } = ConfigurationManager.AppSettings["APICredentials"];
        public static string Endpoints { get; set; } = ConfigurationManager.AppSettings["APIEndpoints"];
        public static string PayloadLocation { get; set; } = ConfigurationManager.AppSettings["APIPayloadLocation"];
        public static int RefreshTime { get; set; } = int.Parse(ConfigurationManager.AppSettings["RefreshTime"]);
        public static string HostName { get; set; } = ConfigurationManager.AppSettings["HostName"];
        public static string[] Games { get; set; } = ConfigurationManager.AppSettings["Games"].Split(',');
        public static string[] Components { get; set; } = ConfigurationManager.AppSettings["Components"].Split(',');
    }
}
