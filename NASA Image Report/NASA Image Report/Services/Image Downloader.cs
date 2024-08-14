// Copyright © - 14/08/2024 - Toby Hunter
using System.Net;

namespace NASAImageReport.Services
{
    internal class ImageDownloader
    {
        public void DownloadImage(string url, string imageId)
        {
            Console.WriteLine("Downloading image {0}", imageId);

            WebClient web = new();

            string fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}temp\{imageId}.png";
            web.DownloadFile(new Uri(url), fileName);

            Console.WriteLine("Downloaded image {0}", imageId);
        }
    }
}
