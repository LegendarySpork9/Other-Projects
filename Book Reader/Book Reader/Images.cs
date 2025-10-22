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
            if (PictureNumber == 1)
            {
                return $@"{Text_Model.ImageLocation}\{Chapter}.png";
            }

            else
            {
                return $@"{Text_Model.ImageLocation}\{Chapter} {PictureNumber}.png";
            }
        }
    }
}
