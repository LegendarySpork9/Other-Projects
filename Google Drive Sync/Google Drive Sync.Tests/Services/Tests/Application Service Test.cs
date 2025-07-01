using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using GoogleDriveSync.Tests.Functions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveSync.Tests.Services.Tests
{
    [TestClass]
    public class ApplicationServiceTest
    {
        // Deletes the Upload Test file from Google Drive.
        [TestInitialize]
        public void Setup()
        {

        }

        // Checks whether the CheckUpdates method returns the expected list.
        [TestMethod]
        public void TestCheckUpdates()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            (List<FileModel> files, bool errored) = _mockApplicationService.Object.CheckUpdates();

            Assert.IsFalse(errored);
            Assert.IsTrue(files.Count > 0);
        }

        // Checks whether the SyncChanges method returns the expected list.
        [TestMethod]
        public void TestSyncChangesUploadCreate()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            string file = $"{AppSettingsModel.LocalFolder}\\Upload Test.txt";
            ApplicationServiceTestFunction.UpdateTestNumber("TestSyncChangesUploadCreate", file);
            FileInfo info = new(file);

            Mock<List<FileModel>> uploadFiles = new();
            uploadFiles.Object.Add(new()
            {
                Id = file,
                Name = "Upload Test",
                Type = "txt",
                Path = "Test",
                Hidden = false,
                Created = info.CreationTime,
                LastModified = info.LastWriteTime
            });

            uploadFiles.Object[0].Changes.Add(new()
            {
                Field = "Path",
                OldValue = "Not Uploaded",
                NewValue = "D:\\System Folders\\Documents\\GitHub\\Other-Projects\\Google Drive Sync\\Google Drive Sync.Tests\\Mocks\\Test",
                Stream = "Down"
            });

            uploadFiles.Object[0].Changes.Add(new()
            {
                Field = "Modified",
                OldValue = "Not Uploaded",
                NewValue = info.LastWriteTime.ToString(),
                Stream = "Down"
            });

            bool errored = _mockApplicationService.Object.SyncChanges(uploadFiles.Object, new());

            Assert.IsFalse(errored);
        }

        // Checks whether the SyncChanges method returns the expected list.
        [TestMethod]
        public void TestSyncChangesUploadMove()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            File.Move($"{AppSettingsModel.LocalFolder}\\Upload Test.txt", $"{AppSettingsModel.LocalFolder}\\Test Two\\Upload Test.txt");
            string file = $"{AppSettingsModel.LocalFolder}\\Test Two\\Upload Test.txt";
            ApplicationServiceTestFunction.UpdateTestNumber("TestSyncChangesUploadMove", file);
            FileInfo info = new(file);

            Mock<List<FileModel>> uploadFiles = new();
            List<FileModel> files = _mockApplicationService.Object.CheckUpdates().Item1;
            uploadFiles.Object.Add(files.Find(c => c.Name == "Upload Test"));

            bool erroredOne = _mockApplicationService.Object.SyncChanges(uploadFiles.Object, new());

            File.Move($"{AppSettingsModel.LocalFolder}\\Test Two\\Upload Test.txt", $"{AppSettingsModel.LocalFolder}\\Upload Test.txt");
            file = $"{AppSettingsModel.LocalFolder}\\Upload Test.txt";
            uploadFiles.Object.Clear();
            files = _mockApplicationService.Object.CheckUpdates().Item1;
            uploadFiles.Object.Add(files.Find(c => c.Name == "Upload Test"));

            bool erroredTwo = _mockApplicationService.Object.SyncChanges(uploadFiles.Object, new());

            Assert.IsFalse(erroredOne);
            Assert.IsFalse(erroredTwo);
        }

        // Checks whether the SyncChanges method returns the expected list.
        [TestMethod]
        public void TestSyncChangesUploadUpdate()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            string file = $"{AppSettingsModel.LocalFolder}\\Upload Test.txt";
            ApplicationServiceTestFunction.UpdateTestNumber("TestSyncChangesUploadUpdate", file);
            FileInfo info = new(file);

            Mock<List<FileModel>> uploadFiles = new();
            List<FileModel> files = _mockApplicationService.Object.CheckUpdates().Item1;
            uploadFiles.Object.Add(files.Find(c => c.Name == "Upload Test"));

            bool errored = _mockApplicationService.Object.SyncChanges(uploadFiles.Object, new());

            Assert.IsFalse(errored);
        }
    }
}
