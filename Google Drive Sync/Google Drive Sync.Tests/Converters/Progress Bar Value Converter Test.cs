// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;
using Moq;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class ProgressBarValueConverterTest
    {
        // Checks whether the GetPBIncreaseValue method returns the expected value.
        [TestMethod]
        public void TestPBIncreaseValue()
        {
            Mock<ProgressBarValueConverter> _mockPorgressBarValueConverter = new();

            int increase = _mockPorgressBarValueConverter.Object.GetPBIncreaseValue(5);

            Assert.AreEqual(20, increase);
        }
    }
}
