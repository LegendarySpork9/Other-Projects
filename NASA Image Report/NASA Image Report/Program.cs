// Copyright © - 14/08/2024 - Toby Hunter
using NASAImageReport.Models;
using NASAImageReport.Services;
using System.Configuration;

namespace NASAImageReport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NASAAPIService _nasaAPIService = new();
            ImageDownloader _imageDownloader = new();
            WordDocumentService _wordDocument = new();

            APIModel api = new()
            {
                APIKey = ConfigurationManager.AppSettings["APIKey"]
            };

            Console.WriteLine("Getting the Mars rover images for the current Mars sol date.");

            List<RoverImageModel> roverImages = _nasaAPIService.GetRoverImages(api);

            Console.WriteLine("Obtained {0} images for sol date {1}.", roverImages.Count, roverImages[0].SolDate);

            if (roverImages.Count != 0)
            {
                Parallel.ForEach(roverImages, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (image, ParallelLoopState) =>
                {
                    _imageDownloader.DownloadImage(image.ImageURL, image.ImageId);
                });
            }

            Console.WriteLine("Getting today's Astronomy Picture of the Day.");

            APODModel apod = _nasaAPIService.GetAPOD(api);

            Console.WriteLine("Obtained today's Astronomy Picture of the Day, {0}.", apod.Title);

            if (apod.ImageURL != null)
            {
                _imageDownloader.DownloadImage(apod.ImageURL, apod.Title);
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Reports";

            Console.WriteLine("Writing report Image Report {0}.docx", today.ToString("dd-MM-yyyy"));

            _wordDocument.CreateDocument(roverImages, apod, $"Image Report {today:dd-MM-yyyy}.docx");

            Console.WriteLine("Report written and put in {0}", filePath);

            DirectoryInfo temp = new($@"{AppDomain.CurrentDomain.BaseDirectory}temp\");

            foreach (FileInfo file in temp.GetFiles())
            {
                file.Delete();
            }

            Console.WriteLine("Temp file folder cleared out.");
        }
    }
}