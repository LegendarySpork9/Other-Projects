namespace ServerSiteAutomation.Models.Data
{
    public class ServerModel
    {
        public string HostName { get; set; }
        public string Game { get; set; }
        public string GameVersion { get; set; }
        public string IPAddress { get; set; }
        public List<StatusModel> Statuses { get; set; }
    }
}
