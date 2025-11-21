// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Converters
{
    public static class ProgressBarValueConverter
    {
        // Returns the value with which to evenly increase the progress bar
        public static int GetPBIncreaseValue(int tasks) => int.Parse($"{100 / tasks}");
    }
}
