// Copyright © - Unpublished - Toby Hunter
namespace ServerStatusCommon.Models.Requests
{
    /// <summary>
    /// Stores the new alert data.
    /// </summary>
    public class NewAlertModel
    {
        public required string Reporter { get; set; }
        public required string Component { get; set; }
        public required string ComponentStatus { get; set; }
        public required string AlertStatus { get; set; }
        public required string HostName { get; set; }
        public required string Game { get; set; }
        public required string GameVersion { get; set; }
    }
}
