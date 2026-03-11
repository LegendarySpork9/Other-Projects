// Copyright © - 05/10/2025 - Toby Hunter
using Moq;
using ServerStatusCommon.Functions;
using ServerStatusCommon.Models;
using ServerStatusCommon.Services;

namespace ServerSite.Tests.Common.Services
{
    [TestClass]
    public class DiscordServiceTest
    {
        private Mock<DiscordService> MockDiscordService;
        private LoggerService Logger;

        // Sets the global variables that the tests to use.
        [TestInitialize]
        public void ConfigureDiscordService()
        {
            MockDiscordService = new Mock<DiscordService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\DiscordService.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        // Checks whether the SetLogger method works as expected.
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

        // Checks whether the SendNotification method works as expected.
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
