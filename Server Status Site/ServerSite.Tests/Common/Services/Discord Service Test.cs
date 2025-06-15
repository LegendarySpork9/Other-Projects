using Moq;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Services;

namespace ServerSite.Tests.Common.Services
{
    [TestClass]
    public class DiscordServiceTest
    {
        private Mock<DiscordService> MockDiscordService;
        private LoggerService Logger;

        [TestInitialize]
        public void ConfigureDiscordService()
        {
            MockDiscordService = new Mock<DiscordService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\DiscordService.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        [TestMethod]
        public void TestSetLogger()
        {
            try
            {
                MockDiscordService.Object.SetLogger(Logger);

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to set logger. Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestSendNotification()
        {
            MockDiscordService.Object.SetLogger(Logger);

            SharedSettingsModel sharedSettings = SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\DiscordService.config")));

            bool successfulSend = MockDiscordService.Object.SendNotification(sharedSettings.RecipientId, "This is a message from a unit test.");

            Assert.IsTrue(successfulSend);
        }
    }
}
