using ServerSiteCommon.Models.Data;

namespace ServerSiteCommon.Models.API
{
    // Stores the server alert's API response.
    public class APIAlertsModel
    {
        public List<AlertModel> Alerts { get; set; } = new List<AlertModel>();
        public bool MultiplePages { get; set; } = false;
        public int PageCount { get; set; } = 0;
        public bool APICalled { get; set; } = false;
    }
}
