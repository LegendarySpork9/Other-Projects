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
    public class DocumentServiceTest
    {
        // Checks whether the GetHasErrored method returns the expected value.
        [TestMethod]
        public void TestHasErrored()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            bool hasErrored = _mockDocumentService.Object.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }

        // Checks whether the GetData method returns the expected list.
        [TestMethod]
        public void TestData()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            List<FileModel> files = _mockDocumentService.Object.GetData();

            Assert.IsTrue(files.Count > 0);
            Assert.AreEqual(8, files.Count);
        }

        // Checks whether the GetFolders method returns the expected arrays.
        [TestMethod]
        public void TestFolders()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            (string[] folderPaths, string[] folderNames) = _mockDocumentService.Object.GetFolders(AppSettingsModel.LocalFolder);

            Assert.IsTrue(folderPaths.Length > 0 && folderNames.Length > 0);
            Assert.AreEqual(3, folderPaths.Length);
            Assert.AreEqual(3, folderNames.Length);
            Assert.IsTrue(folderNames.Contains("Test Four") && folderNames.Contains("Test Three") && folderNames.Contains("Test Two"));
            Assert.IsTrue(folderPaths.Contains($"{AppSettingsModel.LocalFolder}\\Test Four") && folderPaths.Contains($"{AppSettingsModel.LocalFolder}\\Test Three") 
                && folderPaths.Contains($"{AppSettingsModel.LocalFolder}\\Test Two"));
        }

        // Checks whether the GetFiles method returns the expected list.
        [TestMethod]
        public void TestFiles()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            List<FileModel> files = _mockDocumentService.Object.GetFiles(AppSettingsModel.LocalFolder, "Test");

            Assert.IsTrue(files.Count > 0);
            Assert.AreEqual(2, files.Count);
        }
    }
}
