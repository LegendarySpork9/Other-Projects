// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;

namespace GitHubScraper.Implementations
{
    public class SystemClockProvider : IClock
    {
        // Returns the current UTC Date and time.
        public DateTime UtcNow => DateTime.UtcNow;

        // Returns the default date and time.
        public DateTime DefaultDate => new(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    }
}
