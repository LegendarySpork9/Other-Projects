using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GoogleDriveSync.Services
{
    public class GoogleAPIService
    {
        private readonly LoggerService Logger = new LoggerService();
        private readonly string FolderId;
        private readonly string FolderName;

        public GoogleAPIService(string folderId)
        {
            FolderId = folderId;
            FolderName = GetFolderName(folderId);
        }

        private string GetFolderName(string folderId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder name for folder id {folderId}");

            string folderName = string.Empty;

            string[] scopes = { DriveService.Scope.DriveReadonly };
            string applicationName = "Google Drive Sync";

            try
            {
                FileStream stream = new FileStream(AppSettingsModel.Credentials, FileMode.Open, FileAccess.Read);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened stream to Google Drive OAuth Credentials");

                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Generated User Credentials");

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = "mimeType = 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name)";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");

                var response = request.Execute();
                IList<Google.Apis.Drive.v3.Data.File> files = response.Files;

                if (files != null && files.Count > 0)
                {
                    foreach (Google.Apis.Drive.v3.Data.File file in files)
                    {
                        if (file.Id == folderId)
                        {
                            folderName = file.Name;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            if (!string.IsNullOrEmpty(folderName))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained folder name for folder id {folderId}");
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to obtain folder name for folder id {folderId}");
            }

            return folderName;
        }

        public List<FileModel> GetData()
        {
            FolderFunction _folderFunction = new FolderFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file(s) under folder {FolderName ?? FolderId}");

            List<FileModel> googleDrive = new List<FileModel>();
            (string[] folderIds, string[] folderNames) = GetFolders(FolderId);

            googleDrive = GetFiles(FolderId, FolderId, FolderName);

            string pathIds = FolderId;
            string path = FolderName;

            for (int i = 0; i < folderIds.Length; i++)
            {
                pathIds += $@"\{folderIds[i]}";
                path += $@"\{folderNames[i]}";

                TraverseFolders(googleDrive, folderIds[i], pathIds, path);

                pathIds = pathIds.Replace($@"\{folderIds[i]}", "");
                path = path.Replace($@"\{folderNames[i]}", "");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {googleDrive.Count} file(s) under folder {FolderName ?? FolderId}");

            return googleDrive;
        }

        private (string[], string[]) GetFolders(string folderId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {folderId}");

            string[] folderIds = Array.Empty<string>();
            string[] folderNames = Array.Empty<string>();

            string[] scopes = { DriveService.Scope.DriveReadonly };
            string applicationName = "Google Drive Sync";

            try
            {
                FileStream stream = new FileStream(AppSettingsModel.Credentials, FileMode.Open, FileAccess.Read);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened stream to Google Drive OAuth Credentials");

                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Generated User Credentials");

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = $"'{folderId}' in parents and mimeType = 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name)";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Request");

                var response = request.Execute();
                IList<Google.Apis.Drive.v3.Data.File> driveFolders = response.Files;

                if (driveFolders != null && driveFolders.Count > 0)
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                    foreach (Google.Apis.Drive.v3.Data.File folder in driveFolders)
                    {
                        folderIds = folderIds.Append(folder.Id).ToArray();
                        folderNames = folderNames.Append(folder.Name).ToArray();
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the sub folders from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folderIds.Length} folder(s) under folder {folderId}");

            return (folderIds, folderNames);
        }

        private List<FileModel> GetFiles(string folderId, string pathIds, string path)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {folderId}");

            List<FileModel> googleDrive = new List<FileModel>();

            string[] scopes = { DriveService.Scope.DriveReadonly };
            string applicationName = "Google Drive Sync";

            try
            {
                FileStream stream = new FileStream(AppSettingsModel.Credentials, FileMode.Open, FileAccess.Read);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened stream to Google Drive OAuth Credentials");

                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Generated User Credentials");

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = $"'{folderId}' in parents and mimeType != 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name, createdTime, modifiedTime)";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Request");

                var response = request.Execute();
                IList<Google.Apis.Drive.v3.Data.File> driveFiles = response.Files;

                if (driveFiles != null && driveFiles.Count > 0)
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                    foreach (Google.Apis.Drive.v3.Data.File file in driveFiles)
                    {
                        string[] nameSplit = file.Name.Split('.');

                        googleDrive.Add(new FileModel()
                        {
                            Id = file.Id,
                            Name = nameSplit[0],
                            Type = nameSplit[1],
                            PathIds = pathIds,
                            Path = path,
                            Created = DateTime.Parse(file.CreatedTimeRaw),
                            LastModified = DateTime.Parse(file.ModifiedTimeRaw)
                        });
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {googleDrive.Count} file(s) under folder {folderId}");

            return googleDrive;
        }

        private void TraverseFolders(List<FileModel> googleDrive, string folderId, string pathIds, string path)
        {
            (string[] folderIds, string[] folderNames) = GetFolders(folderId);

            googleDrive.AddRange(GetFiles(folderId, pathIds, path));

            for (int i = 0; i < folderIds.Length; i++)
            {
                pathIds += $@"\{folderIds[i]}";
                path += $@"\{folderNames[i]}";

                TraverseFolders(googleDrive, folderIds[i], pathIds, path);

                pathIds = pathIds.Replace($@"\{folderIds[i]}", "");
                path = path.Replace($@"\{folderNames[i]}", "");
            }
        }
    }
}
