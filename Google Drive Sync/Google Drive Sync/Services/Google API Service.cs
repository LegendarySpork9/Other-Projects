// Copyright © - 14/05/2025 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveSync.Services
{
    public class GoogleAPIService
    {
        private readonly LoggerService Logger = new LoggerService();
        private readonly string FolderId;
        private readonly string FolderName;
        private bool HasErrored = false;
        private List<KeyValuePair<string, string>> FolderStore = new List<KeyValuePair<string, string>>();

        // Sets the class's global variables.
        public GoogleAPIService(string folderId)
        {
            FolderId = folderId;
            FolderName = GetFolderName(folderId);
        }

        // Returns the value of the HasErrored variable.
        public bool GetHasErrored() => HasErrored;

        // Resets the HasErrored variable.
        public void ResetHasErrored() => HasErrored = false;

        // Gets the name of the given folder id.
        private string GetFolderName(string folderId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder name for folder id {folderId}");

            string folderName = string.Empty;

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = "mimeType = 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name)";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");

                var response = request.Execute();
                IList<Google.Apis.Drive.v3.Data.File> driveFolders = response.Files;

                if (driveFolders != null && driveFolders.Count > 0)
                {
                    foreach (Google.Apis.Drive.v3.Data.File folder in driveFolders)
                    {
                        if (folder.Id == folderId)
                        {
                            folderName = folder.Name;

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {folderName}");

                            FolderStore.Add(new KeyValuePair<string, string>(folderId, folderName));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to get the files from Google Drive for folder id {folderId}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // Generates credentials from the specified json file.
        private UserCredential GetCredentials()
        {
            UserCredential credential = null;

            string[] scopes = { DriveService.Scope.Drive };

            try
            {
                FileStream credentialsStream = new FileStream(AppSettingsModel.Credentials, FileMode.Open, FileAccess.Read);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened stream to Google Drive OAuth Credentials");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(credentialsStream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore("token.json", true)).Result;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Generated User Credentials");
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to generate user credentials");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to generate user credentials", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return credential;
        }

        // Obtains all the files and folders of the specified directory.
        public List<FileModel> GetData()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file(s) under folder {FolderName ?? FolderId}");

            FolderStore = new List<KeyValuePair<string, string>>();

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

        // Obtains all the folders under a given folder.
        private (string[], string[]) GetFolders(string folderId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {folderId}");

            string[] folderIds = Array.Empty<string>();
            string[] folderNames = Array.Empty<string>();

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.ListRequest request = service.Files.List();
                request.Q = $"'{folderId}' in parents and mimeType = 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name, parents)";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Request");

                var response = request.Execute();
                IList<Google.Apis.Drive.v3.Data.File> driveFolders = response.Files;

                if (driveFolders != null && driveFolders.Count > 0)
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                    foreach (Google.Apis.Drive.v3.Data.File folder in driveFolders)
                    {
                        if (!AppSettingsModel.IgnoreFolders.Contains(folder.Name))
                        {
                            folderIds = folderIds.Append(folder.Id).ToArray();
                            folderNames = folderNames.Append(folder.Name).ToArray();

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Id: {folder.Id}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {folder.Name}");

                            FolderStore.Add(new KeyValuePair<string, string>(folder.Id, $"{folder.Name} ({folder.Parents[0].ToString()})"));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the sub folders from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to get the sub folders from Google Drive for folder id {folderId}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folderIds.Length} folder(s) under folder {folderId}");

            return (folderIds, folderNames);
        }

        // Obtains all the files under a given folder.
        private List<FileModel> GetFiles(string folderId, string pathIds, string path)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {folderId}");

            List<FileModel> googleDrive = new List<FileModel>();

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
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
                        if (!AppSettingsModel.IgnoreFiles.Contains(file.Name))
                        {
                            string[] nameSplit = file.Name.Split('.');

                            googleDrive.Add(new FileModel()
                            {
                                Id = file.Id,
                                Name = nameSplit[0],
                                Type = nameSplit[1],
                                PathIds = pathIds,
                                Path = path,
                                Created = file.CreatedTime.Value,
                                LastModified = file.ModifiedTime.Value
                            });

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Id: {file.Id}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Name: {nameSplit[0]}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Type: {nameSplit[1]}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path Ids: {pathIds}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path: {path}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created Date: {file.CreatedTimeRaw}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Modified Date: {file.ModifiedTimeRaw}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from Google Drive for folder id {folderId}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to get the files from Google Drive for folder id {folderId}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {googleDrive.Count} file(s) under folder {folderId}");

            return googleDrive;
        }

        // Loops through all folders and sub folders to obtain all files.
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

        // Creates a folder under the specified folder.
        private string CreateFolder(string folderName, string parent)
        {
            LoggerFunction _loggerFunction = new LoggerFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating {folderName} folder in Google Drive");

            Google.Apis.Drive.v3.Data.File folder = new Google.Apis.Drive.v3.Data.File();

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { parent }
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: \"{folderName}\", \"{fileMetaData.MimeType}\"");

                FilesResource.CreateRequest createRequest = service.Files.Create(fileMetaData);
                createRequest.Fields = "id";
                folder = createRequest.Execute();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to create {folderName} in Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to create {folderName} in Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!string.IsNullOrWhiteSpace(folder.Id))
            {
                FolderStore.Add(new KeyValuePair<string, string>(folder.Id, $"{folderName} ({parent})"));

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Created {folderName} in Google Drive");
            }

            return folder.Id;
        }

        // Checks if the required folders exist.
        private string CheckFolders(string[] folders)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();

            string parent = AppSettingsModel.DriveFolder;

            for (int x = 1; x < folders.Length; x++)
            {
                string folderStoreValue = $"{folders[x]} ({parent})";
                string folderId = FolderStore.Find(c => c.Value == folderStoreValue).Key;

                if (string.IsNullOrWhiteSpace(folderId))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Warning, $"{folders[x]} folder not found in Google Drive");

                    string newFolderId = CreateFolder(folders[x], parent);

                    if (x != (folders.Length - 1))
                    {
                        parent = newFolderId;
                    }
                }

                else if (x != (folders.Length - 1))
                {
                    parent = folderId;
                }
            }

            return parent;
        }

        // Uploads a new file.
        public void CreateFile(FileModel file)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();
            GoogleDriveConverter _googleDriveConverter = new GoogleDriveConverter();
            LoggerFunction _loggerFunction = new LoggerFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploading {file.Name}.{file.Type} to Google Drive");

            IUploadProgress requestStatus = null;

            string parent = CheckFolders(file.Path.Remove(0, file.Path.IndexOf(',') + 1).Split('\\'));
            string folderStoreValue = $"{_googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { '\\' }, "Right")} ({parent})";

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    Name = $"{file.Name}.{file.Type}",
                    Parents = new List<string> { FolderStore.Find(c => c.Value == folderStoreValue).Key ?? AppSettingsModel.DriveFolder },
                    CreatedTime = file.Created,
                    ModifiedTime = file.LastModified
                };
                
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {_loggerFunction.FormatFileMetaData(fileMetaData, "Create")}");

                FileStream fileStream = new FileStream(file.Id, FileMode.Open);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.CreateMediaUpload createRequest = service.Files.Create(fileMetaData, fileStream, _googleDriveConverter.GetMimeType($".{file.Type}"));
                createRequest.Fields = "id, name";
                requestStatus = createRequest.Upload();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                fileStream.Close();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (requestStatus.Status == UploadStatus.Completed)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploaded {file.Name}.{file.Type} to Google Drive");
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to upload {file.Name}.{file.Type} to Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        // Modifies the existing file.
        public void UpdateFile(FileModel file)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();
            GoogleDriveConverter _googleDriveConverter = new GoogleDriveConverter();
            LoggerFunction _loggerFunction = new LoggerFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploading {file.Name}.{file.Type} to Google Drive");

            IUploadProgress requestStatus = null;

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    ModifiedTime = file.LastModified
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {_loggerFunction.FormatFileMetaData(fileMetaData, "Update")}");

                FileStream fileStream = new FileStream(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"), FileMode.Open);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.UpdateMediaUpload updateRequest = service.Files.Update(fileMetaData, _googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"), fileStream, _googleDriveConverter.GetMimeType($".{file.Type}"));
                updateRequest.Fields = "id, name";
                requestStatus = updateRequest.Upload();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                fileStream.Close();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (requestStatus.Status == UploadStatus.Completed)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploaded {file.Name}.{file.Type} to Google Drive");
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to upload {file.Name}.{file.Type} to Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        // Changes the location of the file.
        public void MoveFile(FileModel file)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();
            GoogleDriveConverter _googleDriveConverter = new GoogleDriveConverter();
            LoggerFunction _loggerFunction = new LoggerFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploading {file.Name}.{file.Type} to Google Drive");

            IUploadProgress requestStatus = null;

            string parent = CheckFolders(file.Path.Remove(0, file.Path.IndexOf(',') + 1).Split('\\'));
            string folderStoreValue = $"{_googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',', '\\' }, "Right")} ({parent})";

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    ModifiedTime = file.LastModified
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {_loggerFunction.FormatFileMetaData(fileMetaData, "Move")}");

                FileStream fileStream = new FileStream(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"), FileMode.Open);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.UpdateMediaUpload moveRequest = service.Files.Update(fileMetaData, _googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"), fileStream, _googleDriveConverter.GetMimeType($".{file.Type}"));
                moveRequest.RemoveParents = _googleDriveFunction.RemoveStringCharacters(file.PathIds, new char[] { '\\' }, "Right");
                moveRequest.AddParents = FolderStore.Find(c => c.Value == folderStoreValue).Key ?? AppSettingsModel.DriveFolder;
                moveRequest.Fields = "id, name, parents";
                requestStatus = moveRequest.Upload();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                fileStream.Close();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (requestStatus != null && requestStatus.Status == UploadStatus.Completed)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploaded {file.Name}.{file.Type} to Google Drive");
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to upload {file.Name}.{file.Type} to Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        // Downloads the file.
        public void DownloadFile(FileModel file)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();
            GoogleDriveConverter _googleDriveConverter = new GoogleDriveConverter();
            FolderFunction _folderFunction = new FolderFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Downloading {file.Name}.{file.Type} to Local Drive");

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                string filePath = _googleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), _googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Path: {filePath}");

                _folderFunction.CheckPath(filePath);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Directories Checked");

                FilesResource.GetRequest downloadRequest = service.Files.Get(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"));
                MemoryStream stream = new MemoryStream();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                downloadRequest.MediaDownloader.ProgressChanged += progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Download File: {progress.BytesDownloaded} bytes");
                            break;

                        case DownloadStatus.Completed:
                            System.IO.File.WriteAllBytes(filePath, stream.ToArray());
                            System.IO.File.SetCreationTime(filePath, file.Created);
                            System.IO.File.SetLastWriteTime(filePath, file.LastModified);
                            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Downloaded {file.Name}.{file.Type} to Local Drive");
                            break;

                        case DownloadStatus.Failed:
                            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to download {file.Name}.{file.Type} to Local Drive");
                            break;
                    }
                };

                downloadRequest.Download(stream);
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Removes the file.
        public void DeleteFile(FileModel file)
        {
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Deleting {file.Name}.{file.Type} from Google Drive");

            try
            {
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredentials(),
                    ApplicationName = "Google Drive Sync",
                });

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");

                FilesResource.DeleteRequest deleteRequest = service.Files.Delete(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"));

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                string result = deleteRequest.Execute();

                if (string.IsNullOrWhiteSpace(result))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Deleted {file.Name}.{file.Type} from Google Drive");
                }

                else
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to delete {file.Name}.{file.Type} from Google Drive. Returned {result}");
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                MessageBox.Show($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
