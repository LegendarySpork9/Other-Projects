// Copyright © - Unpublished - Toby Hunter
using Google.Apis.Auth.OAuth2;

namespace GoogleDriveSync.Abstractions
{
    // Interface for the Google Drive credentials.
    public interface ICredentialProvider
    {
        (UserCredential, bool) GetCredentials();
    }
}
