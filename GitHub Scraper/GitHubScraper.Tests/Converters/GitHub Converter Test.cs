using GitHubScraper.Converters;
using Moq;

namespace GitHubScraper.Tests.Converters
{
    [TestClass]
    public class GitHubConverterTest
    {
        // Tests whether the IsType method returns false when given any value.
        [TestMethod]
        public void TestIsType()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            Assert.IsFalse(_mockGitHubConverter.Object.IsType("Trombone"));
        }

        // Tests whether the IsType method returns true when given "bug".
        [TestMethod]
        public void TestIsTypeBug()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            Assert.IsTrue(_mockGitHubConverter.Object.IsType("bug"));
        }

        // Tests whether the IsType method returns true when given "enhancement".
        [TestMethod]
        public void TestIsTypeEnhancement()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            Assert.IsTrue(_mockGitHubConverter.Object.IsType("enhancement"));
        }

        // Tests whether the IsType method returns true when given "documentation".
        [TestMethod]
        public void TestIsTypeDocumentation()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            Assert.IsTrue(_mockGitHubConverter.Object.IsType("documentation"));
        }

        // Tests whether the GetType method returns the value it's given when given any value.
        [TestMethod]
        public void TestGetType()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            string expected = "Trombone";
            string actual = _mockGitHubConverter.Object.GetType("Trombone");

            Assert.AreEqual(expected, actual);
        }

        // Tests whether the GetType method returns "Bug" when given "bug".
        [TestMethod]
        public void TestGetTypeBug()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            string expected = "Bug";
            string actual = _mockGitHubConverter.Object.GetType("bug");

            Assert.AreEqual(expected, actual);
        }

        // Tests whether the GetType method returns "New Feature" when given "enhancement".
        [TestMethod]
        public void TestGetTypeEnhancement()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            string expected = "New Feature";
            string actual = _mockGitHubConverter.Object.GetType("enhancement");

            Assert.AreEqual(expected, actual);
        }

        // Tests whether the GetType method returns "Documentation" when given "documentation".
        [TestMethod]
        public void TestGetTypeDocumentation()
        {
            Mock<GitHubConverter> _mockGitHubConverter = new();

            string expected = "Documentation";
            string actual = _mockGitHubConverter.Object.GetType("documentation");

            Assert.AreEqual(expected, actual);
        }
    }
}
