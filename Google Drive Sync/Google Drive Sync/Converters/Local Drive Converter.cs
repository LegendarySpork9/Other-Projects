namespace GoogleDriveSync.Converters
{
    public class LocalDriveConverter
    {
        // Returns the name of the object from the full path.
        public string GetObjectName(string objectPath) => objectPath.Remove(0, objectPath.LastIndexOf('\\') + 1);
    }
}
