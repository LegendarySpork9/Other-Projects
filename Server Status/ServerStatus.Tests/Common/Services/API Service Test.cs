// Copyright © - 05/10/2025 - Toby Hunter
using Moq;
using RestSharp;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Models.Data;
using ServerStatusCommon.Services;

namespace ServerStatus.Tests.Common.Services
{
    [TestClass]
    public class APIServiceTest
    {
        /// <summary>
        /// Checks whether the Authorise method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestAuthorise()
        {
            DateTime expiryDate = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.Authorise()).ReturnsAsync(expiryDate);

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object);
            await _apiService.Authorise();

            Assert.AreEqual(expiryDate, _apiService.ExpiryTime);
        }

        /// <summary>
        /// Checks whether the GetUsers method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestUsers()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "[\r\n    {\r\n        \"id\": 1,\r\n        \"username\": \"Test\",\r\n        \"password\": \"HashedString\",\r\n        \"scopes\": [\r\n            \"User\"\r\n        ]\r\n    }\r\n]";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetUsers()).ReturnsAsync(response);

            List<UserModel> expected =
            [
                new()
                {
                    UserId = 1,
                    Username = "Test",
                    Password = "HashedString"
                }
            ];

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            List<UserModel> actual = await _apiService.GetUsers();

            Assert.AreEqual(expected[0].UserId, actual[0].UserId);
            Assert.AreEqual(expected[0].Username, actual[0].Username);
            Assert.AreEqual(expected[0].Password, actual[0].Password);
        }
        
        /// <summary>
        /// Checks whether the GetUserSettings method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettings()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "[\r\n    {\r\n        \"application\": \"Server Status Site\",\r\n        \"settings\": [\r\n            {\r\n                \"id\": 1,\r\n                \"name\": \"DarkMode\",\r\n                \"value\": \"True\"\r\n            },\r\n            {\r\n                \"id\": 2,\r\n                \"name\": \"IsAdmin\",\r\n                \"value\": \"False\"\r\n            },\r\n            {\r\n                \"id\": 3,\r\n                \"name\": \"DiscordName\",\r\n                \"value\": \"UnitTester\"\r\n            }\r\n        ]\r\n    }\r\n]";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetUserSettings(It.IsAny<int>())).ReturnsAsync(response);

            UserModel expected = new()
            {
                UserId = 1,
                Username = "Test",
                Password = "HashedString",
                DiscordName = "UnitTester",
                DarkMode = true,
                Admin = false
            };

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            UserModel actual = await _apiService.GetUserSettings(expected);

            Assert.AreEqual(expected.DiscordName, actual.DiscordName);
            Assert.AreEqual(expected.DarkMode, actual.DarkMode);
            Assert.AreEqual(expected.Admin, actual.Admin);
        }
        
        /// <summary>
        /// Checks whether the GetUserSettingId method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingId()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "[\r\n    {\r\n        \"application\": \"Server Status Site\",\r\n        \"settings\": [\r\n            {\r\n                \"id\": 1,\r\n                \"name\": \"DarkMode\",\r\n                \"value\": \"True\"\r\n            },\r\n            {\r\n                \"id\": 2,\r\n                \"name\": \"IsAdmin\",\r\n                \"value\": \"False\"\r\n            },\r\n            {\r\n                \"id\": 3,\r\n                \"name\": \"DiscordName\",\r\n                \"value\": \"UnitTester\"\r\n            }\r\n        ]\r\n    }\r\n]";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetUserSettings(It.IsAny<int>())).ReturnsAsync(response);

            int expected = 3;

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            int actual = await _apiService.GetUserSettingId(expected, "DiscordName");

            Assert.AreEqual(expected, actual);
        }
        /*
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
            APIStatusModel? status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the GetServerStatus method works with the Server component.
        [TestMethod]
        public void TestServerStatusesServer()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Server Status");
            APIStatusModel? status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the GetServerStatus method works with the Connection component.
        [TestMethod]
        public void TestServerStatusesConnection()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<APIStatusModel> statuses = MockAPIService.Object.GetServerStatuses("Connection Status");
            APIStatusModel? status = statuses.Find(c => c.Server.HostName == "Test PC" && c.Server.Game == "TestGame" && c.Server.GameVersion == "1.0.0");

            Assert.IsNotNull(status);
        }

        // Checks whether the UpdateUserSetting method works as expected.
        [TestMethod]
        public async Task TestUpdateUserSetting()
        {
            MockAPIService.Object.SetLogger(Logger);

            List<UserModel> users = await MockAPIService.Object.GetUsers();
            UserModel? testUser = users.Find(c => c.Username == "UnitTests");

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
            UserModel? user = users.Find(c => c.Username == "UnitTests");

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
            UserModel? testUser = users.Find(c => c.Username == "UnitTests");

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
            UserModel? testUser = users.Find(c => c.Username == "UnitTests");

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
        }*/
    }
}
