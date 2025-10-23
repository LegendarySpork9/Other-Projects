// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerSiteCommon.Models.API
{
    // Stores the server events's API response.
    public class APIStatusModel
    {
        public string Component { get; set; }
        public string Status { get; set; }
        public DateTime DateOccured { get; set; }
        public APIRelatedServerModel Server { get; set; }
    }
}
