// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerStatusCommon.Models.API
{
    /// <summary>
    /// Stores the "server" data from the API responses.
    /// </summary>
    public class APIRelatedServerModel
    {
        public string HostName { get; set; }
        public string Game { get; set; }
        public string GameVersion { get; set; }
    }
}
