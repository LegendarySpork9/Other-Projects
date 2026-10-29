// Copyright © - Unpublished - Toby Hunter
using System.Configuration;

namespace Book_Reader
{
    internal class Images
    {
        // Obtains the images from the file location.
        public static void GetImages(string FileName)
        {
            switch (FileName)
            {
                case "Sensual Adventures": Text_Model.ImageLocation = ConfigurationManager.AppSettings["SAImagesLocation"]; break;
                case "Sexual Adventures": Text_Model.ImageLocation = ConfigurationManager.AppSettings["SImagesLocation"]; break;
            }
        }

        // Gets the image from the location.
        public static string GetNextImage(string Chapter, int PictureNumber)
        {
            string safeChapter = Path.GetFileName(Chapter);

            string filePath;

            if (PictureNumber == 1)
            {
                filePath = Path.Combine(Text_Model.ImageLocation, $"{safeChapter}.png");
            }

            else
            {
                filePath = Path.Combine(Text_Model.ImageLocation, $"{safeChapter} {PictureNumber}.png");
            }

            string resolvedPath = Path.GetFullPath(filePath);

            if (!resolvedPath.StartsWith(Path.GetFullPath(Text_Model.ImageLocation)))
            {
                return string.Empty;
            }

            return resolvedPath;
        }
    }
}
