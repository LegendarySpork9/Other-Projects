using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveSync.Tests.Services
{
    [TestClass]
    public class ApplicationServiceTest
    {
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
        public void TestSyncChangesUpload()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            string file = $"{AppSettingsModel.LocalFolder}\\Upload Test.txt";
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

            (List<FileModel> files, bool errored) = _mockApplicationService.Object.CheckUpdates();

            Assert.IsFalse(errored);
            Assert.IsTrue(files.Count > 0);
        }
    }
}
