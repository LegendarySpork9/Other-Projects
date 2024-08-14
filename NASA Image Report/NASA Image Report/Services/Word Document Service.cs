// Copyright © - 14/08/2024 - Toby Hunter
using WordApplication = Microsoft.Office.Interop.Word.Application;
using WordDocument = Microsoft.Office.Interop.Word.Document;
using WordParagraph = Microsoft.Office.Interop.Word.Paragraph;
using NASAImageReport.Models;

namespace NASAImageReport.Services
{
    internal class WordDocumentService
    {
        public void CreateDocument(List<RoverImageModel> roverImages, APODModel apod, string title)
        {
            object fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}Reports\{title}";
            string imagePath = $@"{AppDomain.CurrentDomain.BaseDirectory}temp\";

            try
            {
                WordApplication word = new();
                WordDocument document = word.Documents.Add();

                WordParagraph paragraph = document.Paragraphs.Add();
                paragraph.Range.Text = $"NASA’s Astronomy Picture of the Day is titled {apod.Title} taken by {apod.Owner}. The following text has been written to explain about the image.";
                paragraph.Range.InsertParagraphAfter();
                
                paragraph = document.Paragraphs.Add();
                paragraph.Range.Text = apod.Description;
                paragraph.Range.Font.Bold = 1;
                paragraph.Range.InsertParagraphAfter();
                paragraph.Range.Font.Bold = 0;
                
                var range = document.Paragraphs.Add().Range;
                range.InlineShapes.AddPicture($"{imagePath}{apod.Title}.png");
                
                Console.WriteLine("Astronomy Picture of the Day section added to document.");
                
                paragraph = document.Paragraphs.Add();
                paragraph.Range.Text = $"The following photos and information come from the Perseverance rover on sol date {roverImages[0].SolDate}.";
                paragraph.Range.InsertParagraphAfter();

                string[] imageIds = Array.Empty<string>();

                foreach (RoverImageModel roverImage in roverImages)
                {
                    if (!imageIds.Contains(roverImage.ImageId))
                    {
                        paragraph = document.Paragraphs.Add();
                        paragraph.Range.Text = $@"Title: {roverImage.Title}
Site: {roverImage.Site}
Date Taken: {roverImage.DateTaken}
Date Received: {roverImage.DateReceived}";
                        paragraph.Range.InsertParagraphAfter();

                        range = document.Paragraphs.Add().Range;
                        range.InlineShapes.AddPicture($"{imagePath}{roverImage.ImageId}.png");

                        Console.WriteLine("{0} section added to document.", roverImage.ImageId);
                        imageIds = imageIds.Append(roverImage.ImageId).ToArray();
                    }
                }

                document.SaveAs2(ref fileName);
                document.Close();
                word.Quit();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
