using GoogleDriveSync.Functions;
using Moq;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class LoggerFunctionTest
    {
        // Checks whether the FormatFileMetaData method returns the expected string values.
        [TestMethod]
        public void TestFileMetaDataCreate()
        {
            Mock<LoggerFunction> _mockLoggerFunction = new();

            string[] result = _mockLoggerFunction.Object.FormatFileMetaData(new Google.Apis.Drive.v3.Data.File
            {
                Name = "Test.txt",
                Parents = new List<string> { "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0" },
                CreatedTime = DateTime.Parse("01/06/1985 11:05:12"),
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            }, "Create").Split(',');

            Assert.IsTrue(result.Length == 4);
            Assert.AreEqual("\"Test.txt\"", result[0].Trim());
            Assert.AreEqual("\"q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0\"", result[1].Trim());
            Assert.AreEqual("\"1985-06-01T10:05:12.000Z\"", result[2].Trim());
            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result[3].Trim());
        }

        // Checks whether the FormatFileMetaData method returns the expected string values.
        [TestMethod]
        public void TestFileMetaDataUpdate()
        {
            Mock<LoggerFunction> _mockLoggerFunction = new();

            string result = _mockLoggerFunction.Object.FormatFileMetaData(new Google.Apis.Drive.v3.Data.File
            {
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            }, "Update");

            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result);
        }

        // Checks whether the FormatFileMetaData method returns the expected string values.
        [TestMethod]
        public void TestFileMetaDataMove()
        {
            Mock<LoggerFunction> _mockLoggerFunction = new();

            string result = _mockLoggerFunction.Object.FormatFileMetaData(new Google.Apis.Drive.v3.Data.File
            {
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            }, "Update");

            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result);
        }
    }
}
