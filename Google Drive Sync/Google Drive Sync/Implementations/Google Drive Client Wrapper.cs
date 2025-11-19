// Copyright © - Unpublished - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using System;
using System.Collections.Generic;

namespace GoogleDriveSync.Implementations
{
    public class GoogleDriveClientWrapper : IGoogleDriveClient
    {
        private readonly ILoggerService _Logger;

        public GoogleDriveClientWrapper(
            ILoggerService _logger)
        {
            _Logger = _logger;
        }

        // Gets the name of the given folder id.
        public (IList<Google.Apis.Drive.v3.Data.File>, bool) GetFolders(UserCredential credential)
        {
            bool hasErrored = false;

            Google.Apis.Drive.v3.Data.FileList response = null;

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Sync",
                });

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = "mimeType = 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name)";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");

                response = request.Execute();
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, "An error occured when trying to get the folders from Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (response.Files ?? new List<Google.Apis.Drive.v3.Data.File>(), hasErrored);
        }
    }
}
