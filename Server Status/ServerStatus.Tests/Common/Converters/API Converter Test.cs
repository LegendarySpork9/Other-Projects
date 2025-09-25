using Moq;
using ServerSiteCommon.Converters;

namespace ServerSite.Tests.Common.Converters
{
    // Checks the GetStatusClass method output with each option.
    [TestClass]
    public class APIConverterTest
    {
        [TestMethod]
        public void TestStatusClassOnline()
        {
            Mock<APIConverter> _mockAPIConverter = new();

            string result = _mockAPIConverter.Object.GetStatusClass("Online");

            Assert.IsTrue(result == "online");
        }

        [TestMethod]
        public void TestStatusClassOffline()
        {
            Mock<APIConverter> _mockAPIConverter = new();

            string result = _mockAPIConverter.Object.GetStatusClass("Offline");

            Assert.IsTrue(result == "offline");
        }

        [TestMethod]
        public void TestStatusClassUnknown()
        {
            Mock<APIConverter> _mockAPIConverter = new();

            string result = _mockAPIConverter.Object.GetStatusClass("Unknown");

            Assert.IsTrue(result == "unknown");
        }

        [TestMethod]
        public void TestStatusClassOther()
        {
            Mock<APIConverter> _mockAPIConverter = new();

            string result = _mockAPIConverter.Object.GetStatusClass("Active");

            Assert.IsTrue(result == "unknown");
        }
    }
}
