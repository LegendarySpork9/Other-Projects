// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerSiteCommon.Models.API
{
    // Stores the body data for the server alert post.
    public class APINewAlertsModel
    {
        public string Reporter { get; set; }
        public string Component { get; set; }
        public string ComponentStatus { get; set; }
        public string AlertStatus { get; set; }
        public string HostName { get; set; }
        public string Game { get; set; }
        public string GameVersion { get; set; }
    }
}
