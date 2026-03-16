// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerStatusCommon.Models.Data
{
    /// <summary>
    /// Stores the individual server alert data.
    /// </summary>
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
