// Copyright © - 05/10/2025 - Toby Hunter
using Moq;
using ServerStatusSite.Converters;

namespace ServerSite.Tests.Site.Converters
{
    [TestClass]
    public class StyleConverterTest
    {
        [TestMethod]
        public void TestTopBarDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTopBarDarkMode(true);

            Assert.AreEqual("background-color: #3E3E3E; border: 1px solid transparent;", style);
        }

        [TestMethod]
        public void TestTopBarDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTopBarDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestTopNavLinkDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTopNavLinkDarkMode(true);

            Assert.AreEqual("color: white;", style);
        }

        [TestMethod]
        public void TestTopNavLinkDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTopNavLinkDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestBodyDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetBodyDarkMode(true);

            Assert.AreEqual("background-color: #313131; color: #A9A9A9;", style);
        }

        [TestMethod]
        public void TestBodyDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetBodyDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestNavMenuDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetNavMenuDarkMode(true);

            Assert.AreEqual("background-color: #4E4E4E; color: white;", style);
        }

        [TestMethod]
        public void TestNavMenuDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetNavMenuDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestTableDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTableDarkMode(true);

            Assert.AreEqual("color: #A9A9A9;", style);
        }

        [TestMethod]
        public void TestTableDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTableDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestFormDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetFormDarkMode(true);

            Assert.AreEqual("form-dark", style);
        }

        [TestMethod]
        public void TestFormDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetFormDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestInputDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetInputDarkMode(true);

            Assert.AreEqual("background-color: #3E3E3E; color: #A9A9A9; border: 1px solid deepskyblue;", style);
        }

        [TestMethod]
        public void TestInputDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetInputDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }

        [TestMethod]
        public void TestTableRowDarkModeTrue()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTableRowDarkMode(true);

            Assert.AreEqual("dark-mode", style);
        }

        [TestMethod]
        public void TestTableRowDarkModeFalse()
        {
            Mock<StyleConverter> _mockStyleConverter = new();

            string style = _mockStyleConverter.Object.GetTableRowDarkMode(false);

            Assert.AreEqual(string.Empty, style);
        }
    }
}
