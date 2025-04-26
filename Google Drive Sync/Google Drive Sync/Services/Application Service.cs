using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using localDriveSync.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveSync.Services
{
    public class ApplicationService
    {
        public void CheckUpdates()
        {
            LoggerService _logger = new LoggerService();
            GoogleAPIService _googleAPIService = new GoogleAPIService(AppSettingsModel.DriveFolder);
            DocumentService _documentService = new DocumentService(AppSettingsModel.LocalFolder);
            FileFunction _fileFunction = new FileFunction();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Checking for updates in {AppSettingsModel.DriveFolder.Length} root folder(s)");

            List<FileModel> googleDrive = _googleAPIService.GetData();
            List<FileModel> localDrive = _documentService.GetData();

            MessageBox.Show("Hello");

            /*

            _fileFunction.CheckForChanges(googleDrives[0], localDrives[0]);*/
        }

        public void SyncChanges()
        {

        }
    }
}
