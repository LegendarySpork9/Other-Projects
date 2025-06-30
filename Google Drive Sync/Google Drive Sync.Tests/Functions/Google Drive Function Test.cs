using GoogleDriveSync.Functions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class GoogleDriveFunctionTest
    {
        [TestMethod]
        public void TestRemoveStringCharactersRight()
        {
            Mock<GoogleDriveFunction> _mockGoogleDriveFunction = new();

            string input = "C:\\GDSTests\\Book Tests";
            string value = _mockGoogleDriveFunction.Object.RemoveStringCharacters(input, new char[] { '\\' }, "Right");

            Assert.IsTrue(value != input);
            Assert.AreEqual("Book Tests", value);
        }
    }
}
