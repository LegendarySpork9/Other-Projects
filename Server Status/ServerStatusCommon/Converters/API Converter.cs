// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerSiteCommon.Converters
{
    public class APIConverter
    {
        // Returns the CSS class of the status.
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
