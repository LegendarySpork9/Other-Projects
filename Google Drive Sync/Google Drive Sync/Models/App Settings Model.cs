// Copyright © - 14/05/2025 - Toby Hunter
using System.Configuration;

namespace GoogleDriveSync.Models
{
    // Stores the settings used by the application.
    public static class AppSettingsModel
    {
        public static string Credentials = ConfigurationManager.AppSettings["CredentialsLocation"].ToString();
        public static string DriveFolder = ConfigurationManager.AppSettings["GoogleDriveFolder"].ToString();
        public static string LocalFolder = ConfigurationManager.AppSettings["LocalDirectory"].ToString();
        public static string[] IgnoreFolders = ConfigurationManager.AppSettings["IgnoreFolders"].ToString().Split(',');
        public static string[] IgnoreFiles = ConfigurationManager.AppSettings["IgnoreFiles"].ToString().Split(',');
    }
}
