// Copyright © - Unpublished - Toby Hunter
using System;

namespace GoogleDriveSync.Abstractions
{
    // Interface for the DateTime object.
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime DefaultDate { get; }
    }
}
