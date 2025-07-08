using Moq;
using ServerSiteAutomation.Services;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Services;

namespace ServerSite.Tests.Automation.Services
{
    [TestClass]
    public class AutomationServerTest
    {
        private Mock<AutomationService> MockAutomationService;
        private LoggerService Logger;

        [TestInitialize]
        public void ConfigureAutomationService()
        {
            MockAutomationService = new Mock<AutomationService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\Automation.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        [TestMethod]
        public void TestSetLogger()
        {
            try
            {
                MockAutomationService.Object.SetLogger(Logger);

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to set logger. Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestSetup()
        {
            MockAutomationService.Object.SetLogger(Logger);

            try
            {
                MockAutomationService.Object.Setup();

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to setup automation service. Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestStart()
        {
            MockAutomationService.Object.SetLogger(Logger);
            MockAutomationService.Object.Setup();

            try
            {
                MockAutomationService.Object.Start();

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to start automation service. Exception: {ex.Message}");
            }
        }
    }
}
