using FluentAssertions;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using System.Configuration;

namespace ServerSite.Tests.Common.Functions
{
    [TestClass]
    public class SharedSettingsLoaderTest
    {
        // Checks the LoadConfig method returns the correct configuration.
        [TestMethod]
        public void TestLoadConfig()
        {
            Configuration result = SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\Test.config"));

            Assert.IsTrue(result.AppSettings.Settings.Count == 2);
        }

        // Checks the LoadConfig method fails to return a configuration.
        [TestMethod]
        public void TestLoadConfigFail()
        {
            Configuration result = SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\Test Two.config"));

            Assert.IsTrue(result.AppSettings.Settings.Count == 0);
        }

        // Checks the LoadSettingsFromConfig method outputs all settings in the config.
        [TestMethod]
        public void TestLoadSettingsFromConfigAutomation()
        {
            SharedSettingsModel expectedSharedSettings = new()
            {
                WebhookURL = "https://thisisatest.com/",
                RecipientId = "test",
                BaseURL = "https://localhost/api",
                Credentials = "Basic TestCreds",
                Endpoints = "Authorisation:/authorise",
                PayloadLocation = "C:\\Server Status Site\\Payload",
                RefreshTime = 5
            };

            SharedSettingsModel result = SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\AutomationTest.config")));

            result.Should().BeEquivalentTo(expectedSharedSettings);
        }

        // Checks the LoadSettingsFromConfig method outputs all settings in the config.
        [TestMethod]
        public void TestLoadSettingsFromConfigReporter()
        {
            SharedSettingsModel expectedSharedSettings = new()
            {
                BaseURL = "https://localhost/api",
                Credentials = "Basic TestCreds",
                Endpoints = "Authorisation:/authorise",
                PayloadLocation = "C:\\Server Status Site\\Payload",
                RefreshTime = 5
            };

            SharedSettingsModel result = SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\ReporterTest.config")));

            result.Should().BeEquivalentTo(expectedSharedSettings);
        }
    }
}
