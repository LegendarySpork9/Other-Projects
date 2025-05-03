using System.IO;

namespace GoogleDriveSync.Functions
{
    internal class FolderFunction
    {
        public void CheckPath(string filePath)
        {
            string fileDirectory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
        }
    }
}
