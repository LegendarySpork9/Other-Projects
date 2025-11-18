// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class GoogleDriveConverterTest
    {
        #region GetMimeType

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeText()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "text/plain";

            string actual = _googleDriveConverter.GetMimeType(".txt");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypePDF()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/pdf";

            string actual = _googleDriveConverter.GetMimeType(".pdf");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeDoc()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/msword";

            string actual = _googleDriveConverter.GetMimeType(".doc");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeDocx()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            string actual = _googleDriveConverter.GetMimeType(".docx");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeXLS()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/vnd.ms-excel";

            string actual = _googleDriveConverter.GetMimeType(".xls");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeXLSX()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            string actual = _googleDriveConverter.GetMimeType(".xlsx");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypePNG()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "image/png";

            string actual = _googleDriveConverter.GetMimeType(".png");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeJPEG()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "image/jpeg";

            string actual = _googleDriveConverter.GetMimeType(".jpeg");

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetMimeType method returns the correct value with the given input.
        [TestMethod]
        public void TestGetMimeTypeDefault()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = "application/octet-stream";

            string actual = _googleDriveConverter.GetMimeType(".xml");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        // Checks whether the GetFilePath method returns the expected file location. 
        [TestMethod]
        public void TestGetFilePath()
        {
            GoogleDriveConverter _googleDriveConverter = new();

            string expected = @"C:\GDSTests\Book Tests\Test.txt";

            string actual = _googleDriveConverter.GetFilePath(@"C:\GDSTests", "Book Tests", "Test.txt");

            Assert.AreEqual(expected, actual);
        }
    }
}
