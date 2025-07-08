namespace GoogleDriveSync.Models
{
    public class ChangeModel
    {
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Stream { get; set; }
    }
}
