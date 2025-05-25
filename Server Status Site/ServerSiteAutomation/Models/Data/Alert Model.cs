namespace ServerSiteAutomation.Models.Data
{
    public class AlertModel
    {
        public int Id { get; set; }
        public DateTime Occured { get; set; }
        public string Server { get; set; }
        public string Reporter { get; set; }
        public string Component { get; set; }
        public string ComponentStatus { get; set; }
        public string AlertStatus { get; set; }
    }
}
