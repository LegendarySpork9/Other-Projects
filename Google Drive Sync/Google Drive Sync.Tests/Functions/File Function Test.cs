using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using Moq;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class FileFunctionTest
    {
        // Checks whether the CompareForChanges method returns the expected changes.
        [TestMethod]
        public void TestCompareForChangesFullUp()
        {
            Mock<FileFunction> _mockFileFunction = new();
            Mock<List<FileModel>> mockGoogleDrive = new();
            Mock<List<FileModel>> mockLocalDrive = new();

            mockGoogleDrive.Object.Add(new()
            {
                Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                Name = "Test",
                Type  = "txt",
                PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0,c7fpJ6Y6eUMpNuqrGmLVhztZOL4l5pxT7",
                Path = "Test Folder,Test Folder Two",
                Created = DateTime.Parse("01/06/1985 11:05:12"),
                LastModified = DateTime.Parse("05/09/1987 13:45:00")
            });

            mockLocalDrive.Object.Add(new()
            {
                Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                Name = "Test",
                Type = "txt",
                Path = "Test Folder",
                Hidden = false,
                Created = DateTime.Parse("01/06/1985 10:55:56"),
                LastModified = DateTime.Parse("03/08/1987 14:23:12")
            });

            List<FileModel> result = _mockFileFunction.Object.CompareForChanges(mockGoogleDrive.Object, mockLocalDrive.Object);

            Assert.IsTrue(result != new List<FileModel>());
            Assert.AreEqual(2, result[0].Changes.Count);
            Assert.IsTrue(result[0].Changes[0].Field == "Path" && result[0].Changes[0].Stream == "Up");
            Assert.IsTrue(result[0].Changes[1].Field == "Modified" && result[0].Changes[1].Stream == "Up");
        }

        // Checks whether the CompareForChanges method returns the expected changes.
        [TestMethod]
        public void TestCompareForChangesPathUp()
        {
            Mock<FileFunction> _mockFileFunction = new();
            Mock<List<FileModel>> mockGoogleDrive = new();
            Mock<List<FileModel>> mockLocalDrive = new();

            mockGoogleDrive.Object.Add(new()
            {
                Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                Name = "Test",
                Type = "txt",
                PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0,c7fpJ6Y6eUMpNuqrGmLVhztZOL4l5pxT7",
                Path = "Test Folder,Test Folder Two",
                Created = DateTime.Parse("01/06/1985 11:05:12"),
                LastModified = DateTime.Parse("05/09/1987 13:45:00")
            });

            mockLocalDrive.Object.Add(new()
            {
                Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                Name = "Test",
                Type = "txt",
                Path = "Test Folder",
                Hidden = false,
                Created = DateTime.Parse("01/06/1985 10:55:56"),
                LastModified = DateTime.Parse("05/09/1987 13:45:00")
            });

            List<FileModel> result = _mockFileFunction.Object.CompareForChanges(mockGoogleDrive.Object, mockLocalDrive.Object);

            Assert.IsTrue(result != new List<FileModel>());
            Assert.AreEqual(1, result[0].Changes.Count);
            Assert.IsTrue(result[0].Changes[0].Field == "Path" && result[0].Changes[0].Stream == "Up");
        }

        // Checks whether the CompareForChanges method returns the expected changes.
        [TestMethod]
        public void TestCompareForChangesModifiedUp()
        {
            Mock<FileFunction> _mockFileFunction = new();
            Mock<List<FileModel>> mockGoogleDrive = new();
            Mock<List<FileModel>> mockLocalDrive = new();

            mockGoogleDrive.Object.Add(new()
            {
                Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                Name = "Test",
                Type = "txt",
                PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0",
                Path = "Test Folder",
                Created = DateTime.Parse("01/06/1985 11:05:12"),
                LastModified = DateTime.Parse("05/09/1987 13:45:00")
            });

            mockLocalDrive.Object.Add(new()
            {
                Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                Name = "Test",
                Type = "txt",
                Path = "Test Folder",
                Hidden = false,
                Created = DateTime.Parse("01/06/1985 10:55:56"),
                LastModified = DateTime.Parse("03/08/1987 14:23:12")
            });

            List<FileModel> result = _mockFileFunction.Object.CompareForChanges(mockGoogleDrive.Object, mockLocalDrive.Object);

            Assert.IsTrue(result != new List<FileModel>());
            Assert.AreEqual(1, result[0].Changes.Count);
            Assert.IsTrue(result[0].Changes[0].Field == "Modified" && result[0].Changes[0].Stream == "Up");
        }

        // Checks whether the CompareForChanges method returns the expected changes.
        [TestMethod]
        public void TestCompareForChangesModifiedDown()
        {
            Mock<FileFunction> _mockFileFunction = new();
            Mock<List<FileModel>> mockGoogleDrive = new();
            Mock<List<FileModel>> mockLocalDrive = new();

            mockGoogleDrive.Object.Add(new()
            {
                Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                Name = "Test",
                Type = "txt",
                PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0",
                Path = "Test Folder",
                Created = DateTime.Parse("01/06/1985 11:05:12"),
                LastModified = DateTime.Parse("03/08/1987 13:45:00")
            });

            mockLocalDrive.Object.Add(new()
            {
                Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                Name = "Test",
                Type = "txt",
                Path = "Test Folder",
                Hidden = false,
                Created = DateTime.Parse("01/06/1985 10:55:56"),
                LastModified = DateTime.Parse("05/09/1987 14:23:12")
            });

            List<FileModel> result = _mockFileFunction.Object.CompareForChanges(mockGoogleDrive.Object, mockLocalDrive.Object);

            Assert.IsTrue(result != new List<FileModel>());
            Assert.AreEqual(1, result[0].Changes.Count);
            Assert.IsTrue(result[0].Changes[0].Field == "Modified" && result[0].Changes[0].Stream == "Down");
        }
    }
}
