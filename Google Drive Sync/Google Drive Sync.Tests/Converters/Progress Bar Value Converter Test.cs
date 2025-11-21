// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class ProgressBarValueConverterTest
    {
        // Checks whether the GetPBIncreaseValue method returns the expected value.
        [TestMethod]
        public void TestGetPBIncreaseValue()
        {
            int expected = 20;

            int actual = ProgressBarValueConverter.GetPBIncreaseValue(5);

            Assert.AreEqual(expected, actual);
        }
    }
}
