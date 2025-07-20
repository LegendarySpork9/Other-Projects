namespace ServerSiteCommon.Converters
{
    public class StandardValues
    {
        // Standard Logger Values.
        public static class LoggerValues
        {
            public const string Debug = "Debug";
            public const string Error = "Error";
            public const string Info = "Info";
            public const string Warning = "Warn";
        }

        // Standard Missing Values.
        public static class MissingValues
        {
            public const string DT = "01/01/1900 00:00:00";
            public const string Integer = "0";

            public const string HostName = "HostNotFound";
            public const string Game = "GameNotFound";
            public const string GameVersion = "GameVersionNotFound";
            public const string IpAddress = "127.0.0.1";

            public const string ResponseContent = "\"{\\\"information\\\":\\\"ResponseContentNotFound\\\"}\"";
            public const string RelatedContent = "\"{\\\"information\\\":\\\"RelatedContentNotFound\\\"}\"";

            public const string AuthEndpoint = "/authorisation";
            public const string BearerToken = "BearerTokenNotObtained";

            public const string UserEndpoint = "/user";
            public const string Username = "UsernameNotObtained";
            public const string Password = "PasswordNotObtained";

            public const string SettingsEndpoint = "/settings";
            public const string SettingStringValue = "ValueNotFound";
            public const string SettingBoolValue = "false";

            public const string ServerEndpoint = "/servers";

            public const string StatusEndpoint = "/status";
            public const string Component = "PC";
            public const string Status = "Offline";

            public const string AlertEndpoint = "/alerts";
            public const string Reporter = "ReporterNotFound";
            public const string AlertStatus = "Reported";
        }
    }
}
