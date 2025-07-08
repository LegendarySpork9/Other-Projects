using System.IO;

namespace GoogleDriveSync.Functions
{
    internal class FolderFunction
    {
        // Creates the local directory if it is not present.
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
