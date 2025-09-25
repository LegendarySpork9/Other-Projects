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

        // Sets the global variables that the tests to use.
        [TestInitialize]
        public void ConfigureAutomationService()
        {
            MockAutomationService = new Mock<AutomationService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\Automation.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        // Checks whether the SetLogger method works as expected.
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

        // Checks whether the Setup method works as expected.
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

        // Checks whether the Start method runs as expected.
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
