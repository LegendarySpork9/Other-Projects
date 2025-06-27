using GoogleDriveSync.Converters;
using Moq;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class GoogleDriveConverterTest
    {
        [TestMethod]
        public void TestMimeTypeText()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".txt");

            Assert.IsTrue(mimeType == "text/plain");
        }

        [TestMethod]
        public void TestMimeTypePDF()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".pdf");

            Assert.IsTrue(mimeType == "application/pdf");
        }

        [TestMethod]
        public void TestMimeTypeDoc()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".doc");

            Assert.IsTrue(mimeType == "application/msword");
        }

        [TestMethod]
        public void TestMimeTypeDocx()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".docx");

            Assert.IsTrue(mimeType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [TestMethod]
        public void TestMimeTypeXLS()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xls");

            Assert.IsTrue(mimeType == "application/vnd.ms-excel");
        }

        [TestMethod]
        public void TestMimeTypeXLSX()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xlsx");

            Assert.IsTrue(mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [TestMethod]
        public void TestMimeTypePNG()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".png");

            Assert.IsTrue(mimeType == "image/png");
        }

        [TestMethod]
        public void TestMimeTypeJPEG()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".jpeg");

            Assert.IsTrue(mimeType == "image/jpeg");
        }

        [TestMethod]
        public void TestMimeTypeDefault()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xml");

            Assert.IsTrue(mimeType == "application/octet-stream");
        }

        [TestMethod]
        public void TestFilePath()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string filePath = _mockGoogleDriveConverter.Object.GetFilePath("C:\\GDSTests", "Book Tests", "Test.txt");

            Assert.IsTrue(filePath == "C:\\GDSTests\\Book Tests\\Test.txt");
        }
    }
}
