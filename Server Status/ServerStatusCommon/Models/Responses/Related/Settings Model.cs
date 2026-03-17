// Copyright © - Unpublished - Toby Hunter
namespace ServerStatusCommon.Models.Responses.Related
{
    /// <summary>
    /// Stores the settings data.
    /// </summary>
    public class SettingsModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
