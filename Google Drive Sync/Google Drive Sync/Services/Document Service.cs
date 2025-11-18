// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleDriveSync.Services
{
    public class DocumentService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IFileMetadataProvider _FileMetadataProvider;
        private readonly IUserNotifier _UserNotifier;
        private readonly string FolderPath;
        private readonly string FolderName;
        private bool HasErrored = false;

        // Sets the class's global variables.
        public DocumentService(ILoggerService _logger, IFileSystem _fileSystem, IFileMetadataProvider _fileMetadataProvider, IUserNotifier _userNotifier, string folder)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _FileMetadataProvider = _fileMetadataProvider;
            _UserNotifier = _userNotifier;
            FolderPath = folder;
            FolderName = folder.Remove(0, folder.LastIndexOf('\\') + 1);
        }

        // Returns the value of the HasErrored variable.
        public bool GetHasErrored() => HasErrored;

        // Obtains all the files and folders of the specified directory.
        public List<FileModel> GetData()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder(s) and file(s) under folder {FolderName}");

            List<FileModel> localDrive = new List<FileModel>();
            (string[] folderPaths, string[] folderNames) = GetFolders(FolderPath);
            
            localDrive = GetFiles(FolderPath, FolderName);

            string path = FolderName;

            for (int i = 0; i < folderPaths.Length; i++)
            {
                path += $@"\{folderNames[i]}";

                TraverseFolders(localDrive, folderPaths[i], path);

                path = path.Replace($@"\{folderNames[i]}", "");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {localDrive.Count} file(s) under folder {FolderName}");

            return localDrive;
        }

        // Obtains all the folders under a given folder.
        private (string[], string[]) GetFolders(string folder)
        {
            LocalDriveConverter _localDriveConverter = new LocalDriveConverter();

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            string[] folderPaths = Array.Empty<string>();
            string[] folderNames = Array.Empty<string>();

            try
            {
                string[] folders = _FileSystem.GetDirectories(folder);

                if (folders.Length > 0)
                {
                    foreach (string folderPath in folders)
                    {
                        if (!AppSettingsModel.IgnoreFolders.Contains(_localDriveConverter.GetObjectName(folderPath)))
                        {
                            folderPaths = folderPaths.Append(folderPath).ToArray();
                            folderNames = folderNames.Append(_localDriveConverter.GetObjectName(folderPath)).ToArray();

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {_localDriveConverter.GetObjectName(folderPath)}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Path: {folderPath}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the sub folders from the Local Drive under folder {_localDriveConverter.GetObjectName(folder)}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                _UserNotifier.ShowMessage($"An error occured when trying to get the sub folders from the Local Drive under folder {_localDriveConverter.GetObjectName(folder)}", "Warning");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folderPaths.Length} folder(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            return (folderPaths, folderNames);
        }

        // Obtains all the files under a given folder.
        private List<FileModel> GetFiles(string folder, string path)
        {
            LocalDriveConverter _localDriveConverter = new LocalDriveConverter();

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            List<FileModel> localDrive = new List<FileModel>();

            try
            {
                string[] filePaths = _FileSystem.GetFiles(folder);

                if (filePaths.Length > 0)
                {
                    foreach (string filePath in filePaths)
                    {
                        if (!AppSettingsModel.IgnoreFiles.Contains(_localDriveConverter.GetObjectName(filePath)) && !filePath.Contains("~$e"))
                        {
                            (DateTime created, DateTime modified, bool hidden) = GetFileInformation(filePath);

                            string[] nameSplit = _localDriveConverter.GetObjectName(filePath).Split('.');

                            localDrive.Add(new FileModel()
                            {
                                Id = filePath,
                                Name = nameSplit[0],
                                Type = nameSplit[1],
                                Path = path,
                                Hidden = hidden,
                                Created = created,
                                LastModified = modified
                            });

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Id: {filePath}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Name: {nameSplit[0]}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Type: {nameSplit[1]}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path: {path}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hidden: {hidden}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created Date: {created}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Modified Date: {modified}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from the Local Drive under folder {_localDriveConverter.GetObjectName(folder)}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                _UserNotifier.ShowMessage($"An error occured when trying to get the files from the Local Drive under folder {_localDriveConverter.GetObjectName(folder)}", "Warning");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {localDrive.Count} file(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            return localDrive;
        }

        // Obtains specific information about the file.
        private (DateTime, DateTime, bool) GetFileInformation(string file)
        {
            LocalDriveConverter _localDriveConverter = new LocalDriveConverter();

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file {_localDriveConverter.GetObjectName(file)}");

            (DateTime created, DateTime modified, bool hidden) = _FileMetadataProvider.GetFileInformation(file);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained file information for file {_localDriveConverter.GetObjectName(file)}");

            return (created, modified, hidden);
        }

        // Loops through all folders and sub folders to obtain all files.
        private void TraverseFolders(List<FileModel> localDrive, string folder, string path)
        {
            (string[] folderPaths, string[] folderNames) = GetFolders(folder);

            localDrive.AddRange(GetFiles(folder, path));

            for (int i = 0; i < folderPaths.Length; i++)
            {
                path += $@"\{folderNames[i]}";

                TraverseFolders(localDrive, folderPaths[i], path);

                path = path.Replace($@"\{folderNames[i]}", "");
            }
        }

        // Delets the given file.
        public void DeleteFile(string file) => _FileSystem.DeleteFile(file);

        // Unblocks the given file.
        public void UnblockFile(string file)
        {
            string adsPath = $"{file}:Zone.Identifier";

            if (_FileSystem.FileExists(adsPath))
            {
                _FileSystem.DeleteFile(adsPath);
            }
        }

        // Hides the given file if it was already hidden.
        public void HideFile(string file, bool hidden)
        {
            if (_FileSystem.FileExists(file))
            {
                if (hidden)
                {
                    _FileSystem.SetAttributes(file, _FileSystem.GetAttributes(file) | FileAttributes.Hidden);
                }

                else
                {
                    _FileSystem.SetAttributes(file, _FileSystem.GetAttributes(file) & ~FileAttributes.Hidden);
                }
            }
        }
    }
}
