// Copyright © - Unpublished - Toby Hunter
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;

namespace GoogleDriveSync.Abstractions
{
    // Interface for the Google Drive operations.
    public interface IGoogleDriveClient
    {
        (IList<Google.Apis.Drive.v3.Data.File>, bool) GetFolders(UserCredential credentials);
    }
}
