// Copyright © - 14/08/2024 - Toby Hunter
namespace NASAImageReport.Models
{
    internal class APODModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Owner { get; set; } = "NASA";
        public string? ImageURL { get; set; }
    }
}
