// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerSiteCommon.Models
{
    // Stores the settings shared by the three applications.
    public class SharedSettingsModel
    {
        public string Domain { get; set; }
        public string WebhookURL { get; set; }
        public string RecipientId { get; set; }
        public string RecipientIds { get; set; }
        public bool SendAlerts { get; set; }
        public string BaseURL { get; set; }
        public string Credentials { get; set; }
        public string Endpoints { get; set; }
        public string PayloadLocation { get; set; }
        public int RefreshTime { get; set; }
    }
}
