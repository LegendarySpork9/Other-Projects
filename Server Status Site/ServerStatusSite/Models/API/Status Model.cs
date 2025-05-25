namespace ServerStatusSite.Models.API
{
    public class APIStatusModel
    {
        public string Component { get; set; }
        public string Status { get; set; }
        public DateTime DateOccured { get; set; }
        public APIRelatedServerModel Server { get; set; }
    }
}
