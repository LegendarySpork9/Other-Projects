using System.Configuration;

namespace ServerSiteReporter.Models
{
    // Stores the app specific settings.
    public static class AppSettingsModel
    {
        public static string HostName { get; set; } = ConfigurationManager.AppSettings["HostName"];
        public static string[] Games { get; set; } = ConfigurationManager.AppSettings["Games"].Split(',');
        public static string[] Components { get; set; } = ConfigurationManager.AppSettings["Components"].Split(',');
    }
}
