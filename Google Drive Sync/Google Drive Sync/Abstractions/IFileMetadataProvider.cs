// Copyright © - Unpublished - Toby Hunter
using System;

namespace GoogleDriveSync.Abstractions
{
    // Interface for file metadata operations.
    public interface IFileMetadataProvider
    {
        (DateTime, DateTime, bool) GetFileInformation(string file);
    }
}
