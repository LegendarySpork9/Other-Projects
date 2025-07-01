namespace GoogleDriveSync.Functions
{
    public class LoggerFunction
    {
        // Returns a sting of the meta data for a Google Drive file.
        public string FormatFileMetaData(Google.Apis.Drive.v3.Data.File fileMetaData, string method)
        {
            string formattedFMD = string.Empty;

            if (fileMetaData != null)
            {
                if (method == "Create")
                {
                    formattedFMD = $"\"{fileMetaData.Name}\", \"{fileMetaData.Parents[0]}\", \"{fileMetaData.CreatedTimeRaw}\", \"{fileMetaData.ModifiedTimeRaw}\"";
                }

                else if (method == "Update" || method == "Move")
                {
                    formattedFMD = $"\"{fileMetaData.ModifiedTimeRaw}\"";
                }
            }

            return formattedFMD;
        }
    }
}
