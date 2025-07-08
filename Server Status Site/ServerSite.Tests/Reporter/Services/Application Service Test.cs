using Moq;
using ServerSiteReporter.Services;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Services;

namespace ServerSite.Tests.Reporter.Services
{
    [TestClass]
    public class ApplicationServiceTest
    {
        private Mock<ApplicationService> MockApplicationService;
        private LoggerService Logger;

        [TestInitialize]
        public void ConfigureAutomationService()
        {
            MockApplicationService = new Mock<ApplicationService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\Reporter.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        [TestMethod]
        public void TestSetLogger()
        {
            try
            {
                MockApplicationService.Object.SetLogger(Logger);

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
            MockApplicationService.Object.SetLogger(Logger);

            try
            {
                MockApplicationService.Object.Setup();

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
            MockApplicationService.Object.SetLogger(Logger);
            MockApplicationService.Object.Setup();

            try
            {
                MockApplicationService.Object.Start();

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to start application service. Exception: {ex.Message}");
            }
        }
    }
}
