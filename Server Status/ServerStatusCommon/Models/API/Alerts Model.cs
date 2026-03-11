// Copyright © - 05/10/2025 - Toby Hunter
using ServerStatusCommon.Models.Data;

namespace ServerStatusCommon.Models.API
{
    /// <summary>
    /// Stores the server alert's API response.
    /// </summary>
    public class APIAlertsModel
    {
        public List<AlertModel> Alerts { get; set; } = [];
        public bool MultiplePages { get; set; } = false;
        public int PageCount { get; set; } = 0;
        public bool APICalled { get; set; } = false;
    }
}
