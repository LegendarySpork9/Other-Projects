using GitHubScraper.Models.Related;

namespace GitHubScraper.Models
{
    // Stores information about the release.
    internal class ReleaseModel
    {
        public string? Repository { get; set; }
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required UserModel Author { get; set; }
        public required string Body { get; set; }
        public int NumberOfAssets { get; set; } = 0;
        public required bool Draft { get; set; }
        public required DateTime Created_At { get; set; }
        public required DateTime Updated_At { get; set; }
        public DateTime? Published_At { get; set; }
        public required List<AssetModel> Assets { get; set; } = [];
    }
}
