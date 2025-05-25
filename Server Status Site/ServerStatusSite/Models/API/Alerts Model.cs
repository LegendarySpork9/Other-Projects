using ServerStatusSite.Models.Data;

namespace ServerStatusSite.Models.API
{
    public class APIAlertsModel
    {
        public List<AlertModel> Alerts { get; set; } = new List<AlertModel>();
        public bool MultiplePages { get; set; } = false;
        public int PageCount { get; set; } = 0;
        public bool APICalled { get; set; } = false;
    }
}
