// Copyright © - 14/08/2024 - Toby Hunter
namespace NASAImageReport.Models
{
    internal class RoverImageModel
    {
        public string? ImageId { get; set; }
        public int? SolDate { get; set; }
        public string? Title { get; set; }
        public int? Site { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateTaken { get; set; }
        public string? ImageURL { get; set; }
    }
}
