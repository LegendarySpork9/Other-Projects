using GitHubScraper.Models;
using GitHubScraper.Services;
using Moq;
using System.Configuration;

namespace GitHubScraper.Tests.Services
{
    [TestClass]
    public class ApplicationServiceTest
    {
        // Checks whether the configuration passes with all configuration values provided.
        [TestMethod]
        public void TestSetupPass()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsTrue(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration passes with all configuration values provided except the workflows.
        [TestMethod]
        public void TestSetupPassNoWorkflow()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsTrue(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when all the configuration values are missing.
        [TestMethod]
        public void TestSetupAllFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = null;
            AppSettingsModel.Repositories = null;
            AppSettingsModel.BearerToken = null;
            AppSettingsModel.ConnectionString = null;
            AppSettingsModel.SQLFiles = null;

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when the owner configuration value is missing.
        [TestMethod]
        public void TestSetupOwnerFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = null;
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when the repository configuration value is missing.
        [TestMethod]
        public void TestSetupRepositoryFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = null;
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when the token configuration value is missing.
        [TestMethod]
        public void TestSetupTokenFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = null;
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when the connection string configuration value is missing.
        [TestMethod]
        public void TestSetupConnectionStringFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = null;
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }

        // Checks whether the configuration fails when the SQL files configuration value is missing.
        [TestMethod]
        public void TestSetupSQLFail()
        {
            Mock<ApplicationService> _mockApplicationService = new();

            AppSettingsModel.Owner = ConfigurationManager.AppSettings["Owner"];
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.Workflows = ConfigurationManager.AppSettings["Workflows"];
            AppSettingsModel.BearerToken = ConfigurationManager.AppSettings["BearerToken"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = null;

            Assert.IsFalse(_mockApplicationService.Object.Setup());
        }
    }
}
