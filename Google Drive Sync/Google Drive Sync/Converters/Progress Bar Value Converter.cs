namespace GoogleDriveSync.Converters
{
    internal class ProgressBarValueConverter
    {
        public int GetPBIncreaseValue(int tasks) => int.Parse($"{100 / tasks}");
    }
}
