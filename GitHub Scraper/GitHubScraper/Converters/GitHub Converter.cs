// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Converters
{
    public static class GitHubConverter
    {
        // Returns the query parameters for a given endpoint.
        public static string GetQuery(string endpoint)
        {
            return endpoint switch
            {
                "/issues" => "?state=all&sort=updated&per_page=100",
                _ => string.Empty
            };
        }

        // Returns whether the label is an issue type.
        public static bool IsType(string label)
        {
            return label switch
            {
                "bug" => true,
                "enhancement" => true,
                "documentation" => true,
                _ => false
            };
        }

        // Returns the correct issue type for the label.
        public static string GetType(string label)
        {
            return label switch
            {
                "bug" => "Bug",
                "enhancement" => "New Feature",
                "documentation" => "Documentation",
                _ => label
            };
        }
    }
}
