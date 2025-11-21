// Copyright © - Unpublished - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using GoogleDriveSync.Models;
using System.Collections.Generic;

namespace GoogleDriveSync.Abstractions
{
    // Interface for the Google Drive operations.
    public interface IGoogleDriveClient
    {
        void CreateGoogleDriveService(UserCredential credentials);
        (IList<File>, bool) GetFolders(string folderId = null);
        (IList<File>, bool) GetFiles(string folderId);
        (File, bool) CreateFolder(string folderName, string parent);
        (IUploadProgress, bool) CreateFile(FileModel file, string parent);
        (IUploadProgress, bool) UpdateFile(FileModel file);
        (IUploadProgress, bool) MoveFile(FileModel file, string oldParent, string newParent);
        bool DownloadFile(FileModel file, string filePath);
        bool DeleteFile(FileModel file);
    }
}
