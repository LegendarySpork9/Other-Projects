// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using GoogleDriveSync.Tests.Functions;
using Moq;

namespace GoogleDriveSync.Tests.Services
{
    [TestClass]
    public class GoogleAPIServiceTest
    {
        // Checks whether the GetHasErrored method returns the expected value.
        [TestMethod]
        public void TestHasErrored()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            bool hasErrored = _mockGoogleAPIService.Object.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }

        // Checks whether the GetData method returns the expected list.
        [TestMethod]
        public void TestData()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            List<FileModel> files = _mockGoogleAPIService.Object.GetData();

            Assert.IsTrue(files.Count > 0);
            Assert.AreEqual(10, files.Count);
        }

        // Checks whether the CreateFile method can successfully create the file.
        [TestMethod]
        public void TestCreate()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            _mockGoogleAPIService.Object.ResetHasErrored();

            string filePath = $"{AppSettingsModel.LocalFolder}\\Google Test.txt";
            GoogleAPIServiceTestFunction.UpdateTestNumber("TestCreate", filePath);
            FileInfo info = new(filePath);
            FileModel file = new()
            {
                Id = filePath,
                Name = "Google Test",
                Type = "txt",
                Path = "Test",
                Hidden = false,
                Created = info.CreationTime,
                LastModified = info.LastWriteTime
            };

            _mockGoogleAPIService.Object.CreateFile(file);

            Assert.IsFalse(_mockGoogleAPIService.Object.GetHasErrored());
        }

        // Checks whether the UpdateFile method can successfully update the file.
        [TestMethod]
        public void TestUpdate()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            _mockGoogleAPIService.Object.ResetHasErrored();

            string filePath = $"{AppSettingsModel.LocalFolder}\\Google Test.txt";
            GoogleAPIServiceTestFunction.UpdateTestNumber("TestUpdate", filePath);
            FileInfo info = new(filePath);

            List<FileModel> files = _mockGoogleAPIService.Object.GetData();
            FileModel file = files.Find(c => c.Name == "Google Test");
            file.Id = $"{file.Id},{filePath}";
            file.LastModified = info.LastWriteTime;

            _mockGoogleAPIService.Object.UpdateFile(file);

            Assert.IsFalse(_mockGoogleAPIService.Object.GetHasErrored());
        }

        // Checks whether the MoveFile method can successfully move the file.
        [TestMethod]
        public void TestMove()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            string filePath = MoveFunction.MoveFile("Google Test.txt");
            GoogleAPIServiceTestFunction.UpdateTestNumber("TestMove", filePath);

            FileInfo info = new(filePath);
            List<FileModel> files = _mockGoogleAPIService.Object.GetData();
            FileModel file = files.Find(c => c.Name == "Google Test");
            file.Id = $"{file.Id},{filePath}";
            file.Path = $"{file.Path},{filePath.Replace(AppSettingsModel.LocalFolder, "Test").Replace("\\Google Test.txt", "")}";
            file.LastModified = info.LastWriteTime;

            _mockGoogleAPIService.Object.ResetHasErrored();
            _mockGoogleAPIService.Object.MoveFile(file);

            Assert.IsFalse(_mockGoogleAPIService.Object.GetHasErrored());
        }

        // Checks whether the DownloadFile method can successfully download the file.
        [TestMethod]
        public void TestDownload()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            string filePath = $"{AppSettingsModel.LocalFolder}\\Google Test.txt";
            File.Delete(filePath);

            List<FileModel> files = _mockGoogleAPIService.Object.GetData();
            FileModel file = files.Find(c => c.Name == "Google Test");

            _mockGoogleAPIService.Object.ResetHasErrored();
            _mockGoogleAPIService.Object.DownloadFile(file);
            GoogleAPIServiceTestFunction.UpdateTestNumber("TestDownload", filePath);

            Assert.IsFalse(_mockGoogleAPIService.Object.GetHasErrored());
        }

        // Checks whether the DeleteFile method can successfully download the file.
        [TestMethod]
        public void TestDelete()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            string filePath = $"{AppSettingsModel.LocalFolder}\\Google Test.txt";
            GoogleAPIServiceTestFunction.UpdateTestNumber("TestDelete", filePath);

            List<FileModel> files = _mockGoogleAPIService.Object.GetData();
            FileModel file = files.Find(c => c.Name == "Google Test");

            _mockGoogleAPIService.Object.ResetHasErrored();
            _mockGoogleAPIService.Object.DeleteFile(file);

            Assert.IsFalse(_mockGoogleAPIService.Object.GetHasErrored());
        }
    }
}
