// Copyright © - Unpublished - Toby Hunter
namespace GitHubScraper.Converters
{
    public static class GitHubConverter
    {
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
