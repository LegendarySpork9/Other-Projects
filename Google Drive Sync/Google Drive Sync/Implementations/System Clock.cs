// Copyright © - Unpublished - Toby Hunter
using GoogleDriveSync.Abstractions;
using System;

namespace GoogleDriveSync.Implementations
{
    public class SystemClock : IClock
    {
        // Returns the current UTC Date and time.
        public DateTime UtcNow => DateTime.UtcNow;

        // Returns the default date and time.
        public DateTime DefaultDate => new DateTime(1900, 01, 01);
    }
}
