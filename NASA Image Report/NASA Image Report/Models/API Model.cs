// Copyright © - 14/08/2024 - Toby Hunter
namespace NASAImageReport.Models
{
    internal class APIModel
    {
        public string? APIKey { get; set; }
        public string RoverURL = "https://mars.nasa.gov/rss/api/";
        public string APODURL = "https://api.nasa.gov/planetary/apod";
    }
}
