namespace ServerSiteAutomation.Converters
{
    public class APIConverter
    {
        public string GetStatusClass(string status)
        {
            return status switch
            {
                "Online" => "online",
                "Offline" => "offline",
                "Unknown" => "unknown",
                _ => "unknown"
            };
        }
    }
}
