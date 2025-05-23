namespace ServerStatusSite.Models
{
    public class AppSettingsModel
    {
        public string Domain { get; set; }
        public string BaseURL { get; set; }
        public string Credentials { get; set; }
        public string Endpoints { get; set; }
        public string PayloadLocation { get; set; }
        public int RefreshTime { get; set; }
    }
}
