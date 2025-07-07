using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveSync.Tests.Services
{
    [TestClass]
    public class GoogleAPIServiceTest
    {
        // Checks whether the GetHasErrored method returns the expected value.
        [TestMethod]
        public void TestHasErrored()
        {
            Mock<GoogleAPIService> _mockGoogleAPIService = new(AppSettingsModel.DriveFolder);

            bool hasErrored = _mockGoogleAPIService.Object.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }


    }
}
