using GoogleDriveSync.Converters;
using Moq;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class GoogleDriveConverterTest
    {
        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeText()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".txt");

            Assert.AreEqual("text/plain", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypePDF()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".pdf");

            Assert.AreEqual("application/pdf", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeDoc()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".doc");

            Assert.AreEqual("application/msword", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeDocx()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".docx");

            Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeXLS()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xls");

            Assert.AreEqual("application/vnd.ms-excel", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeXLSX()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xlsx");

            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypePNG()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".png");

            Assert.AreEqual("image/png", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeJPEG()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".jpeg");

            Assert.AreEqual("image/jpeg", mimeType);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestMimeTypeDefault()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string mimeType = _mockGoogleDriveConverter.Object.GetMimeType(".xml");

            Assert.AreEqual("application/octet-stream", mimeType);
        }

        // Checks whether the GetFilePath method returns the expected file location. 
        [TestMethod]
        public void TestFilePath()
        {
            Mock<GoogleDriveConverter> _mockGoogleDriveConverter = new();

            string filePath = _mockGoogleDriveConverter.Object.GetFilePath("C:\\GDSTests", "Book Tests", "Test.txt");

            Assert.AreEqual("C:\\GDSTests\\Book Tests\\Test.txt", filePath);
        }
    }
}
