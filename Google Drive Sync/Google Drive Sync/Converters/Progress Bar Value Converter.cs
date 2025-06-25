namespace GoogleDriveSync.Converters
{
    internal class ProgressBarValueConverter
    {
        // Returns the value with which to evenly increase the progress bar
        public int GetPBIncreaseValue(int tasks) => int.Parse($"{100 / tasks}");
    }
}
