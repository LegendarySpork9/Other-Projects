using System.Configuration;

namespace Book_Reader
{
    internal class Images
    {
        public static void GetImages(string FileName)
        {
            switch (FileName)
            {
                case "Sensual Adventures": Text_Model.ImageLocation = ConfigurationManager.AppSettings["SAImagesLocation"]; break;
                case "Sexual Adventures": Text_Model.ImageLocation = ConfigurationManager.AppSettings["SImagesLocation"]; break;
            }
        }

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
