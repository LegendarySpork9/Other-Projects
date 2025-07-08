using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;

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
            Assert.AreEqual(10, files.Count);
        }

        // Checks if the DeleteFile method can successfully delete files.
        [TestMethod]
        public void TestDelete()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            string file = $@"{AppSettingsModel.LocalFolder}/Local Delete Test.txt";
            string content = File.ReadAllText(file);

            _mockDocumentService.Object.DeleteFile(file);

            Assert.IsFalse(File.Exists(file));

            File.WriteAllText(file, content);
        }

        // Checks if the HideFile method can hide the file.
        [TestMethod]
        public void TestHide()
        {
            Mock<DocumentService> _mockDocumentService = new(AppSettingsModel.LocalFolder);

            string file = $@"{AppSettingsModel.LocalFolder}/Hide Test.txt";

            _mockDocumentService.Object.HideFile(file, true);

            FileAttributes attributes = new FileInfo(file).Attributes;

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                Assert.IsTrue(true);
            }

            _mockDocumentService.Object.HideFile(file, false);
        }
    }
}
