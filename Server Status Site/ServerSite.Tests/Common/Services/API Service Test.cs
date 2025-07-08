using Moq;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Functions;

namespace ServerSite.Tests.Common.Services
{
    [TestClass]
    public class APIServiceTest
    {
        private Mock<APIService> MockAPIService;
        private LoggerService Logger;

        [TestInitialize]
        public void ConfigureAPIService()
        {
            MockAPIService = new Mock<APIService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\APIService.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }

        [TestMethod]
        public void TestSetLogger()
        {
            try
            {
                MockAPIService.Object.SetLogger(Logger);

                Assert.IsTrue(true);
            }

            catch (Exception ex)
            {
                Assert.Fail($"Failed to set logger. Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestAuthorise()
        {
            MockAPIService.Object.SetLogger(Logger);
            MockAPIService.Object.Authorise();

            Assert.IsTrue(MockAPIService.Object.ExpiryTime != DateTime.Parse("01/01/0001 00:00:00"));
        }

        [TestMethod]
        public void TestUsers()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();

            Assert.IsTrue(users.Count > 0);
        }

        [TestMethod]
        public void TestUserSettings()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                testUser = MockAPIService.Object.GetUserSettings(testUser);

                Assert.IsNotNull(testUser.DiscordName);
                Assert.IsTrue(testUser.Admin);
                Assert.IsTrue(testUser.DarkMode);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        [TestMethod]
        public void TestUserSettingId()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                int userSettingId = MockAPIService.Object.GetUserSettingId(testUser.UserId, "DiscordName");

                Assert.IsTrue(userSettingId != 0);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        [TestMethod]
        public void TestServers()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<ServerModel> servers = MockAPIService.Object.GetServers();

            Assert.IsTrue(servers.Count > 0);
        }

        [TestMethod]
        public void TestServerStatusesPC()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("PC Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void TestServerStatusesHamachi()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Hamachi Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void TestServerStatusesServer()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Server Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void TestUpdateUserSetting()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                DateTime testDate = DateTime.UtcNow;
                int userSettingId = MockAPIService.Object.GetUserSettingId(testUser.UserId, "DiscordName");
                bool updated = MockAPIService.Object.UpdateUserSettings(userSettingId, $"UnitTester {testDate}");
                
                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();
            UserModel user = users.Find(c => c.Username == "UnitTestTestUser");

            if (user != null)
            {
                Mock<HashFunction> _mockHashFunction = new();

                string testDate = DateTime.UtcNow.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
                user.Password = _mockHashFunction.Object.HashString($"Password{testDate}");

                bool updated = MockAPIService.Object.UpdateUser(user);

                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        [TestMethod]
        public void TestAlerts()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = MockAPIService.Object.GetAlerts(1);

            Assert.IsTrue(alerts.Alerts.Count > 0);
        }

        [TestMethod]
        public void TestAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = MockAPIService.Object.GetAlerts(1);
            AlertModel alert = MockAPIService.Object.GetAlert(alerts.Alerts[0].Id);

            Assert.IsTrue(alert.Id != 0);
        }

        [TestMethod]
        public void TestUpdateAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = MockAPIService.Object.GetAlerts(1);
            AlertModel alert = MockAPIService.Object.GetAlert(alerts.Alerts[0].Id);

            if (alert.Id > 0)
            {
                if (alert.AlertStatus == "Reported")
                {
                    alert.AlertStatus = "Investigating";
                }

                else
                {
                    alert.AlertStatus = "Reported";
                }

                bool updated = MockAPIService.Object.UpdateAlert(alert.Id, alert.AlertStatus);

                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find an alert.");
            }
        }

        [TestMethod]
        public void TestRegisterAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                testUser = MockAPIService.Object.GetUserSettings(testUser);

                APINewAlertsModel alert = new()
                {
                    Reporter = testUser.DiscordName,
                    Component = "PC Status",
                    ComponentStatus = "Unknown",
                    AlertStatus = "Reported",
                    HostName = "Test PC",
                    Game = "TestGame",
                    GameVersion = "1.0.0"
                };

                bool registered = MockAPIService.Object.RegisterAlert(alert);

                Assert.IsTrue(registered);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        [TestMethod]
        public void TestRegisterServerEvent()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIStatusModel status = new()
            {
                Component = "PC Status",
                Status = "Unknown",
                DateOccured = DateTime.UtcNow,
                Server = new()
                {
                    HostName = "Test PC",
                    Game = "TestGame",
                    GameVersion = "1.0.0"
                }
            };

            bool registered = MockAPIService.Object.RegisterServerEvent(status);

            Assert.IsTrue(registered);
        }
    }
}
