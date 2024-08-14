// Copyright © - 14/08/2024 - Toby Hunter
using NASAImageReport.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;

namespace NASAImageReport.Services
{
    internal class NASAAPIService
    {
        public List<RoverImageModel> GetRoverImages(APIModel api)
        {
            ImageDownloader _imageDownloader = new();

            List<RoverImageModel> roverImages = new();
            int pageNumber = 1;
            bool searchComplete = false;

            while (!searchComplete)
            {
                RestClient rest = new(api.RoverURL);
                RestRequest request = new();
                request.Method = Method.Get;
                request.AddParameter("feed", "raw_images");
                request.AddParameter("category", "mars2020");
                request.AddParameter("feedtype", "json");
                request.AddParameter("num", "100");
                request.AddParameter("page", pageNumber);
                request.AddParameter("order", "sol");

                try
                {
                    RestResponse response = rest.Execute(request);

                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(response.Content))
                    {
                        JObject apiResponse = JObject.Parse(response.Content);
                        JArray imageArray = JArray.Parse(apiResponse["images"].ToString());
                        int marsSol = int.Parse(JObject.Parse(imageArray[0].ToString())["sol"].ToString());

                        foreach (JObject imageData in imageArray)
                        {
                            int recordMarsSol = int.Parse(imageData["sol"].ToString());

                            if (recordMarsSol == marsSol)
                            {
                                RoverImageModel image = new()
                                {
                                    ImageId = imageData["imageid"].ToString(),
                                    SolDate = int.Parse(imageData["sol"].ToString()),
                                    Title = imageData["title"].ToString(),
                                    Site = int.Parse(imageData["site"].ToString()),
                                    DateReceived = DateTime.Parse(imageData["date_received"].ToString(), null, DateTimeStyles.RoundtripKind),
                                    DateTaken = DateTime.Parse(imageData["date_taken_utc"].ToString(), null, DateTimeStyles.RoundtripKind),
                                    ImageURL = imageData["image_files"]["full_res"].ToString()
                                };

                                roverImages.Add(image);
                            }

                            else
                            {
                                searchComplete = true;
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (!searchComplete)
                {
                    pageNumber++;
                }
            }

            return roverImages;
        }

        public APODModel GetAPOD(APIModel api)
        {
            ImageDownloader _imageDownloader = new();

            APODModel apod = new();

            RestClient rest = new(api.APODURL);
            RestRequest request = new();
            request.Method = Method.Get;
            request.AddParameter("api_key", api.APIKey);

            try
            {
                RestResponse response = rest.Execute(request);

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(response.Content))
                {
                    JObject imageData = JObject.Parse(response.Content);

                    APODModel image = new()
                    {
                        Title = imageData["title"].ToString(),
                        Description = imageData["explanation"].ToString(),
                        ImageURL = imageData["url"].ToString()
                    };

                    if (imageData["copyright"] != null)
                    {
                        image.Owner = imageData["copyright"].ToString().Replace("\n", "");
                    }

                    apod = image;
                }
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return apod;
        }
    }
}
