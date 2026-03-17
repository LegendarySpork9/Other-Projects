// Copyright © - Unpublished - Toby Hunter
using ServerStatusCommon.Models.Responses.Related;

namespace ServerStatusCommon.Models.Responses
{
    /// <summary>
    /// Stores the server event API response.
    /// </summary>
    public class ServerEventModel
    {
        public required string Component { get; set; }
        public required string Status { get; set; }
        public required DateTime DateOccured { get; set; }
        public required RelatedServerModel Server { get; set; }
    }
}
