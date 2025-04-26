using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace localDriveSync.Services
{
    public class DocumentService
    {
        /*LoggerService Logger = new();

        public FolderModel GetData(FolderModel localDrive)
        {
            FolderFunction _folderFunction = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder(s) and file(s) under folder {localDrive.Name}");

            localDrive.Folders = GetFolders(localDrive.Id);
            localDrive.Files = GetFiles(localDrive.Id, localDrive.Name);

            string path = localDrive.Name;

            foreach (FolderModel folder in localDrive.Folders)
            {
                path += $@"\{folder.Name}";

                _folderFunction.TraverseLocalFolders(folder, path);

                path = path.Replace($@"\{folder.Name}", "");
            }

            (int folders, int files) = _folderFunction.GetCounts();

            folders += localDrive.Folders.Count;
            files += localDrive.Files.Count;

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folders} file(s) and {files} folder(s) under folder {localDrive.Name}");

            return localDrive;
        }

        public List<FolderModel> GetFolders(string folder)
        {
            LocalDriveConverter _localDriveConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            List<FolderModel> folders = new();

            string[] folderPaths = Directory.GetDirectories(folder);

            if (folderPaths.Length > 0)
            {
                foreach (string folderPath in folderPaths)
                {
                    if (!AppSettingsModel.IgnoreFolders.Contains(_localDriveConverter.GetObjectName(folderPath)))
                    {
                        folders.Add(new FolderModel()
                        {
                            Id = folderPath,
                            Name = _localDriveConverter.GetObjectName(folderPath)
                        });
                    }
                }
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folders.Count} folder(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            return folders;
        }

        public List<FileModel> GetFiles(string folder, string path)
        {
            LocalDriveConverter _localDriveConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            List<FileModel> files = new();

            string[] filePaths = Directory.GetFiles(folder);

            if (filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    if (!AppSettingsModel.IgnoreFiles.Contains(_localDriveConverter.GetObjectName(filePath)) && !filePath.Contains("~$e"))
                    {
                        (DateTime created, DateTime modified, bool hidden) = GetFileInformation(filePath);

                        files.Add(new FileModel()
                        {
                            Id = filePath,
                            Name = _localDriveConverter.GetObjectName(filePath),
                            Path = path,
                            Created = created,
                            LastModified = modified,
                            Hidden = hidden
                        });
                    }
                }
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {files.Count} file(s) under folder {_localDriveConverter.GetObjectName(folder)}");

            return files;
        }

        private (DateTime, DateTime, bool) GetFileInformation(string file)
        {
            LocalDriveConverter _localDriveConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file {_localDriveConverter.GetObjectName(file)}");

            FileInfo info = new(file);
            FileAttributes attributes = info.Attributes;

            DateTime created = info.CreationTime;
            DateTime modified = info.LastWriteTime;
            bool hidden = false;

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                hidden = true;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained file information for file {_localDriveConverter.GetObjectName(file)}");

            return (created, modified, hidden);
        }*/
    }
}
