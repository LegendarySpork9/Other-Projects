using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;

namespace GoogleDriveSync.Services
{
    public class ApplicationService
    {
        private readonly GoogleAPIService GoogleAPI = new GoogleAPIService(AppSettingsModel.DriveFolder);
        public event Action<int> ProgressChanged;

        // Increases the value of the pogress bar.
        private void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        // Runs the process of checking for updates.
        public (List<FileModel>, bool) CheckUpdates()
        {
            LoggerService _logger = new LoggerService();
            ProgressBarValueConverter _progressBarValueConverter = new ProgressBarValueConverter();
            DocumentService _documentService = new DocumentService(AppSettingsModel.LocalFolder);
            FileFunction _fileFunction = new FileFunction();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Checking for updates in root folder");

            int progress = 0;
            int increaseValue = _progressBarValueConverter.GetPBIncreaseValue(4);

            List<FileModel> googleDrive = GoogleAPI.GetData();
            bool hasErrored = GoogleAPI.GetHasErrored();

            progress += increaseValue;
            OnProgressChanged(progress);

            List<FileModel> localDrive = _documentService.GetData();
            hasErrored = _documentService.GetHasErrored();

            progress += increaseValue;
            OnProgressChanged(progress);

            List<FileModel> files = _fileFunction.CompareForChanges(googleDrive, localDrive);

            progress += increaseValue;
            OnProgressChanged(progress);

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Checked for updates in root folder");

            return (files, hasErrored);
        }

        // Runs the process of updating the files selected.
        public bool SyncChanges(List<FileModel> uploadFiles, List<FileModel> downloadFiles)
        {
            LoggerService _logger = new LoggerService();
            ProgressBarValueConverter _progressBarValueConverter = new ProgressBarValueConverter();
            DocumentService _documentService = new DocumentService(AppSettingsModel.LocalFolder);
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();
            GoogleDriveConverter _googleDriveConverter = new GoogleDriveConverter();
            FileFunction _fileFunction = new FileFunction();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Syncing {uploadFiles.Count + downloadFiles.Count} files(s) to root folder");

            bool hasErrored = false;

            int progress = 0;
            int increaseValue = _progressBarValueConverter.GetPBIncreaseValue(uploadFiles.Count + downloadFiles.Count + 1);

            GoogleAPI.ResetHasErrored();

            foreach (FileModel file in uploadFiles)
            {
                if (file.Changes[0].OldValue == "Not Uploaded")
                {
                    GoogleAPI.CreateFile(file);
                }

                else
                {
                    if (file.Changes[0].Field == "Path")
                    {
                        GoogleAPI.MoveFile(file);
                    }

                    else
                    {
                        GoogleAPI.UpdateFile(file);
                    }
                }

                hasErrored = GoogleAPI.GetHasErrored();

                progress += increaseValue;
                OnProgressChanged(progress);
            }

            foreach (FileModel file in downloadFiles)
            {
                _documentService.HideFile(_googleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), _googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"), false);

                if (file.Changes[0].Field == "Path")
                {
                    GoogleAPI.DownloadFile(file);

                    _documentService.DeleteFile(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"));
                }

                else
                {
                    GoogleAPI.DownloadFile(file);
                }

                hasErrored = GoogleAPI.GetHasErrored();

                _documentService.UnblockFile(_googleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), _googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"));
                _documentService.HideFile(_googleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), _googleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"), file.Hidden);

                progress += increaseValue;
                OnProgressChanged(progress);
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Synced {uploadFiles.Count + downloadFiles.Count} files(s) to root folder");

            return hasErrored;
        }
    }
}
