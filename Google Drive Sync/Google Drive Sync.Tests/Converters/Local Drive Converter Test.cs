// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;
using Moq;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class LocalDriveConverterTest
    {
        // Checks whether the GetObjectName method returns the expected file name.
        [TestMethod]
        public void TestObjectName()
        {
            Mock<LocalDriveConverter> _mockLocalDriveConverter = new();

            string fileName = _mockLocalDriveConverter.Object.GetObjectName("C:\\GDSTests\\Book Tests\\Test.txt");

            Assert.AreEqual("Test.txt", fileName);
        }
    }
}
