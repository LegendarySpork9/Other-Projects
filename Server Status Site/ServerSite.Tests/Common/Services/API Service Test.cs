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

        // Sets the global variables that the tests to use.
        [TestInitialize]
        public void ConfigureAPIService()
        {
            MockAPIService = new Mock<APIService>(SharedSettingsLoader.LoadSettingsFromConfig(SharedSettingsLoader.LoadConfig(Path.Combine(Directory.GetCurrentDirectory().Replace(@"bin\Debug\net8.0", ""), @"Mocks\Configs\APIService.config"))));
            Logger = new();
            Logger.ChangeIdentifier("UnitTest");
        }
        
        // Checks whether the SetLogger method works as expected.
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

        // Checks whether the Authorise method works as expected.
        [TestMethod]
        public void TestAuthorise()
        {
            MockAPIService.Object.SetLogger(Logger);
            MockAPIService.Object.Authorise();

            Assert.IsTrue(MockAPIService.Object.ExpiryTime != DateTime.Parse("01/01/0001 00:00:00"));
        }

        // Checks whether the AuthoriseAsync method works as expected.
        [TestMethod]
        public async Task TestAuthoriseAsync()
        {
            MockAPIService.Object.SetLogger(Logger);
            await MockAPIService.Object.AuthoriseAsync();

            Assert.IsTrue(MockAPIService.Object.ExpiryTime != DateTime.Parse("01/01/0001 00:00:00"));
        }

        // Checks whether the GetUsers method works as expected.
        [TestMethod]
        public async Task TestUsers()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();

            Assert.IsTrue(users.Count > 0);
        }

        // Checks whether the GetUserSettings method works as expected.
        [TestMethod]
        public async Task TestUserSettings()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                testUser = await MockAPIService.Object.GetUserSettings(testUser);

                Assert.IsNotNull(testUser.DiscordName);
                Assert.IsFalse(testUser.Admin);
                Assert.IsFalse(testUser.DarkMode);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        // Checks whether the GetUserSettingId method works as expected.
        [TestMethod]
        public async Task TestUserSettingId()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
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

        // Checks whether the GetServers method works as expected.
        [TestMethod]
        public void TestServers()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<ServerModel> servers = MockAPIService.Object.GetServers();

            Assert.IsTrue(servers.Count > 0);
        }

        // Checks whether the GetServerStatus method works with the PC component.
        [TestMethod]
        public void TestServerStatusesPC()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("PC Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the GetServerStatus method works with the Hamachi component.
        [TestMethod]
        public void TestServerStatusesHamachi()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Hamachi Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the GetServerStatus method works with the Server component.
        [TestMethod]
        public void TestServerStatusesServer()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Server Status");
            APIStatusModel status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the UpdateUserSetting method works as expected.
        [TestMethod]
        public async Task TestUpdateUserSetting()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                DateTime testDate = DateTime.UtcNow;
                int userSettingId = MockAPIService.Object.GetUserSettingId(testUser.UserId, "DiscordName");
                bool updated = await MockAPIService.Object.UpdateUserSettings(userSettingId, $"UnitTester {testDate}");
                
                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        // Checks whether the UpdateUser method works as expected.
        [TestMethod]
        public async Task TestUpdateUser()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel user = users.Find(c => c.Username == "UnitTests");

            if (user != null)
            {
                Mock<HashFunction> _mockHashFunction = new();

                string testDate = DateTime.UtcNow.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
                user.Password = _mockHashFunction.Object.HashString($"Password{testDate}");

                bool updated = await MockAPIService.Object.UpdateUser(user);

                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        // Checks whether the GetAlerts method works as expected.
        [TestMethod]
        public void TestAlerts()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = MockAPIService.Object.GetAlerts(1);

            Assert.IsTrue(alerts.Alerts.Count > 0);
        }

        // Checks whether the GetAlertsAsync method works as expected.
        [TestMethod]
        public async Task TestAlertsAsync()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = await MockAPIService.Object.GetAlertsAsync(1);

            Assert.IsTrue(alerts.Alerts.Count > 0);
        }

        // Checks whether the GetAlert method works as expected.
        [TestMethod]
        public void TestAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = MockAPIService.Object.GetAlerts(1);
            AlertModel alert = MockAPIService.Object.GetAlert(alerts.Alerts[0].Id);

            Assert.IsTrue(alert.Id != 0);
        }

        // Checks whether the UpdateAlert method works as expected.
        [TestMethod]
        public async Task TestUpdateAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            APIAlertsModel alerts = await MockAPIService.Object.GetAlertsAsync(1);
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

                bool updated = await MockAPIService.Object.UpdateAlert(alert.Id, alert.AlertStatus);

                Assert.IsTrue(updated);
            }

            else
            {
                Assert.Fail("Failed to find an alert.");
            }
        }

        // Checks whether the RegisterAlertAsync method works as expected.
        [TestMethod]
        public async Task TestRegisterAlert()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                testUser = await MockAPIService.Object.GetUserSettings(testUser);

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

        // Checks whether the RegisterAlertAsync method works as expected.
        [TestMethod]
        public async Task TestRegisterAlertAsync()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel testUser = users.Find(c => c.Username == "UnitTests");

            if (testUser != null)
            {
                testUser = await MockAPIService.Object.GetUserSettings(testUser);

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

                bool registered = await MockAPIService.Object.RegisterAlertAsync(alert);

                Assert.IsTrue(registered);
            }

            else
            {
                Assert.Fail("Failed to find test user.");
            }
        }

        // Checks whether the RegisterServerEvent method works as expected.
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
