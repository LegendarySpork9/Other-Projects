using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using localDriveSync.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveSync.Services
{
    public class ApplicationService
    {
        public void CheckUpdates()
        {
            LoggerService _logger = new LoggerService();
            GoogleAPIService _googleAPIService = new GoogleAPIService(AppSettingsModel.DriveFolder);
            LocalDriveConverter _localDriveConverter = new LocalDriveConverter();
            DocumentService _documentService = new DocumentService();
            FileFunction _fileFunction = new FileFunction();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Checking for updates in {AppSettingsModel.DriveFolder.Length} root folder(s)");

            List<FileModel> googleDrive = _googleAPIService.GetData();

            /*List<FolderModel> localDrives = new();

            foreach (string localFolder in AppSettingsModel.LocalFolder)
            {
                FolderModel localDrive = new()
                {
                    Id = localFolder,
                    Name = _localDriveConverter.GetObjectName(localFolder)
                };

                localDrive = _documentService.GetData(localDrive);

                localDrives.Add(localDrive);
            }

            _fileFunction.CheckForChanges(googleDrives[0], localDrives[0]);*/
        }

        public void SyncChanges()
        {

        }
    }
}
