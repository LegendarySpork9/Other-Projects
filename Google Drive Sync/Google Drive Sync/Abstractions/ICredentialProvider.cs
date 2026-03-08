// Copyright © - Unpublished - Toby Hunter
using Google.Apis.Auth.OAuth2;

namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the Google Drive credentials.
    /// </summary>
    public interface ICredentialProvider
    {
        (UserCredential, bool) GetCredentials();
    }
}
