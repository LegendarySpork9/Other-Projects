// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Models.Related
{
    /// <summary>
    /// Stores information about the user.
    /// </summary>
    public class UserModel
    {
        public string? Login { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset? Date { get; set; }
    }
}
