namespace GoogleDriveSync.Converters
{
    public class LocalDriveConverter
    {
        public string GetObjectName(string objectPath) => objectPath.Remove(0, objectPath.LastIndexOf('\\') + 1);
    }
}
