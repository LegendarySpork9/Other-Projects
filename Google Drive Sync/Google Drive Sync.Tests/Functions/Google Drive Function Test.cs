using GoogleDriveSync.Functions;
using Moq;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class GoogleDriveFunctionTest
    {
        // Checks whether the RemoveStringCharacters method returns the expected string.
        [TestMethod]
        public void TestRemoveStringCharactersRightBackSlash()
        {
            Mock<GoogleDriveFunction> _mockGoogleDriveFunction = new();

            string input = "Book Tests\\Test Folder\\Test Folder Two";
            string value = _mockGoogleDriveFunction.Object.RemoveStringCharacters(input, new char[] { '\\' }, "Right");

            Assert.IsTrue(value != input);
            Assert.AreEqual("Test Folder Two", value);
        }

        // Checks whether the RemoveStringCharacters method returns the expected string.
        [TestMethod]
        public void TestRemoveStringCharactersRightComma()
        {
            Mock<GoogleDriveFunction> _mockGoogleDriveFunction = new();

            string input = "C:\\GDSTests\\Book Tests,rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE";
            string value = _mockGoogleDriveFunction.Object.RemoveStringCharacters(input, new char[] { ',' }, "Right");

            Assert.IsTrue(value != input);
            Assert.AreEqual("rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE", value);
        }

        // Checks whether the RemoveStringCharacters method returns the expected string.
        [TestMethod]
        public void TestRemoveStringCharactersRightBoth()
        {
            Mock<GoogleDriveFunction> _mockGoogleDriveFunction = new();

            string input = "C:\\GDSTests\\Book Tests,Book Tests\\Test Folder\\Test Folder Two";
            string value = _mockGoogleDriveFunction.Object.RemoveStringCharacters(input, new char[] { ',', '\\' }, "Right");

            Assert.IsTrue(value != input);
            Assert.AreEqual("Test Folder Two", value);
        }

        // Checks whether the RemoveStringCharacters method returns the expected string.
        [TestMethod]
        public void TestRemoveStringCharactersLeft()
        {
            Mock<GoogleDriveFunction> _mockGoogleDriveFunction = new();

            string input = "C:\\GDSTests\\Book Tests,Book Tests\\Test Folder\\Test Folder Two";
            string value = _mockGoogleDriveFunction.Object.RemoveStringCharacters(input, new char[] { ',' }, "Left");

            Assert.IsTrue(value != input);
            Assert.AreEqual("C:\\GDSTests\\Book Tests", value);
        }
    }
}
