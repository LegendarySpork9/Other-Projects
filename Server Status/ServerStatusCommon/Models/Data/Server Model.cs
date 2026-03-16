// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerStatusCommon.Models.Data
{
    /// <summary>
    /// Stores the server data.
    /// </summary>
    public class ServerModel
    {
        public string HostName { get; set; }
        public string Game { get; set; }
        public string GameVersion { get; set; }
        public ConnectionModel Connection { get; set; }
        public DowntimeModel? Downtime { get; set; }
        public List<StatusModel> Statuses { get; set; }
    }
}
