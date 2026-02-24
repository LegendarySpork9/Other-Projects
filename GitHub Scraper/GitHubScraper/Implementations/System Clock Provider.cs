// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;

namespace GitHubScraper.Implementations
{
    public class SystemClockProvider : IClock
    {
        // Returns the current UTC Date and time.
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
