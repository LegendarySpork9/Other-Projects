// Copyright © - Unpublished - Toby Hunter
namespace ServerStatusCommon.Models.Requests
{
    /// <summary>
    /// Stores the new event data.
    /// </summary>
    public class NewEventModel
    {
        public required string Component { get; set; }
        public required string Status { get; set; }
        public required string HostName { get; set; }
        public required string Game { get; set; }
        public required string GameVersion { get; set; }
    }
}
