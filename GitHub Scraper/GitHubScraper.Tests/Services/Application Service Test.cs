// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Abstractions;
using GitHubScraper.Models;
using GitHubScraper.Services;
using Moq;

namespace GitHubScraper.Tests.Services
{
    [TestClass]
    public class ApplicationServiceTest
    {
        /// <summary>
        /// Checks whether the configuration passes with all configuration values provided.
        /// </summary>
        [TestMethod]
        public void TestSetupPass()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = "UnitTester";
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration passes with all configuration values provided except the workflows.
        /// </summary>
        [TestMethod]
        public void TestSetupPassNoWorkflow()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = AppSettingsModel.Owner = "UnitTester";;
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when all the configuration values are missing.
        /// </summary>
        [TestMethod]
        public void TestSetupAllFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = null;
            AppSettingsModel.Repositories = null;
            AppSettingsModel.BearerToken = null;
            AppSettingsModel.ConnectionString = null;
            AppSettingsModel.SQLFiles = null;

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when the owner configuration value is missing.
        /// </summary>
        [TestMethod]
        public void TestSetupOwnerFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = null;
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when the repository configuration value is missing.
        /// </summary>
        [TestMethod]
        public void TestSetupRepositoryFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = AppSettingsModel.Owner = "UnitTester";;
            AppSettingsModel.Repositories = null;
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when the token configuration value is missing.
        /// </summary>
        [TestMethod]
        public void TestSetupTokenFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = AppSettingsModel.Owner = "UnitTester";;
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = null;
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when the connection string configuration value is missing.
        /// </summary>
        [TestMethod]
        public void TestSetupConnectionStringFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = AppSettingsModel.Owner = "UnitTester";;
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = null;
            AppSettingsModel.SQLFiles = @"C:\SQL";

            Assert.IsTrue(_applicationService.Setup());
        }

        /// <summary>
        /// Checks whether the configuration fails when the SQL files configuration value is missing.
        /// </summary>
        [TestMethod]
        public void TestSetupSQLFail()
        {
            Mock<ILoggerService> _mockLogger = new();

            ApplicationService _applicationService = new(_mockLogger.Object);

            AppSettingsModel.Owner = AppSettingsModel.Owner = "UnitTester";;
            AppSettingsModel.Repositories = "Unit-Test";
            AppSettingsModel.Workflows = "Unit Test.yml";
            AppSettingsModel.BearerToken = "This is a token";
            AppSettingsModel.ConnectionString = "This is a connection string";
            AppSettingsModel.SQLFiles = null;

            Assert.IsTrue(_applicationService.Setup());
        }
    }
}
