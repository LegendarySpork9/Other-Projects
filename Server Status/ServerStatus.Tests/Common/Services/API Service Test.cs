// Copyright © - 05/10/2025 - Toby Hunter
using Moq;
using RestSharp;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Models.API;
using ServerStatusCommon.Models.Data;
using ServerStatusCommon.Services;
using ServerStatusSite.Components.Pages.Alerts;

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
        public async Task TestGetUsers()
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
        public async Task TestGetUserSettings()
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
        public async Task TestGetUserSettingId()
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
        
        /// <summary>
        /// Checks whether the GetServers method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestGetServers()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContentServer = "[\r\n    {\r\n        \"id\": 1,\r\n        \"hostName\": \"LocalHost\",\r\n        \"game\": \"Minecraft\",\r\n        \"gameVersion\": \"1.7.10\",\r\n        \"connection\": {\r\n            \"ipAddress\": \"127.0.0.1\",\r\n            \"port\": 25565\r\n        },\r\n        \"downtime\": {\r\n            \"time\": \"02:00:00\"\r\n        },\r\n        \"isActive\": true\r\n    }\r\n]";
            RestResponse responseServer = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContentServer
            };
            string responseContentStatus = "[\r\n    {\r\n        \"component\": \"component\",\r\n        \"status\": \"Offline\",\r\n        \"dateOccured\": \"2025-10-03T20:02:53.023Z\",\r\n        \"server\": {\r\n            \"hostName\": \"LocalHost\",\r\n            \"game\": \"Minecraft\",\r\n            \"gameVersion\": \"1.7.10\"\r\n        }\r\n    }\r\n]";
            RestResponse responseStatus = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContentStatus
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetServers()).ReturnsAsync(responseServer);
            _mockAPIClient.Setup(api => api.GetServerStatuses(It.IsAny<string>())).ReturnsAsync(responseStatus);

            List<ServerModel> expected =
            [
                new()
                {
                    HostName = "LocalHost",
                    Game = "Minecraft",
                    GameVersion = "1.7.10",
                    Connection = new()
                    {
                        IPAddress = "127.0.0.1",
                        Port = 25565
                    },
                    Downtime = null,
                    Statuses =
                    [
                        new()
                        {
                            Status = "Offline",
                            StatusClass = "offline"
                        },
                        new()
                        {
                            Status = "Offline",
                            StatusClass = "offline"
                        },
                        new()
                        {
                            Status = "Offline",
                            StatusClass = "offline"
                        }
                    ]
                }
            ];

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            List<ServerModel> actual = await _apiService.GetServers();

            Assert.AreEqual(expected[0].HostName, actual[0].HostName);
            Assert.AreEqual(expected[0].Game, actual[0].Game);
            Assert.AreEqual(expected[0].GameVersion, actual[0].GameVersion);

            for (int x = 0; x < expected[0].Statuses.Count; x++)
            {
                Assert.AreEqual(expected[0].Statuses[x].Status, actual[0].Statuses[x].Status);
                Assert.AreEqual(expected[0].Statuses[x].StatusClass, actual[0].Statuses[x].StatusClass);
            }
        }

        /// <summary>
        /// Checks whether the GetServerStatus method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatuses()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "[\r\n    {\r\n        \"component\": \"component\",\r\n        \"status\": \"Offline\",\r\n        \"dateOccured\": \"2025-10-03T20:02:53.023Z\",\r\n        \"server\": {\r\n            \"hostName\": \"LocalHost\",\r\n            \"game\": \"Minecraft\",\r\n            \"gameVersion\": \"1.7.10\"\r\n        }\r\n    }\r\n]";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetServerStatuses(It.IsAny<string>())).ReturnsAsync(response);

            List<APIStatusModel> expected =
            [
                new()
                {
                    Component = "component",
                    Status = "Offline",
                    DateOccured = new(2025, 10, 03, 20, 02, 53, DateTimeKind.Utc),
                    Server = new()
                    {
                        HostName = "LocalHost",
                        Game = "Minecraft",
                        GameVersion = "1.7.10"
                    }
                }
            ];

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            List<APIStatusModel> actual = await _apiService.GetServerStatuses("component");

            Assert.AreEqual(expected[0].Component, actual[0].Component);
            Assert.AreEqual(expected[0].Status, actual[0].Status);
            Assert.AreEqual(expected[0].DateOccured, actual[0].DateOccured);
            Assert.AreEqual(expected[0].Server.HostName, actual[0].Server.HostName);
            Assert.AreEqual(expected[0].Server.Game, actual[0].Server.Game);
            Assert.AreEqual(expected[0].Server.GameVersion, actual[0].Server.GameVersion);
        }
        
        /// <summary>
        /// Checks whether the UpdateUserSetting method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUserSetting()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "{\r\n    \"id\": 1,\r\n    \"name\": \"DarkMode\",\r\n    \"value\": \"False\"\r\n}";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.UpdateUserSettings(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(response);

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            bool updated = await _apiService.UpdateUserSettings(1, "False");

            Assert.IsTrue(updated);
        }
        
        /// <summary>
        /// Checks whether the UpdateUser method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUser()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "{\r\n    \"id\": 1,\r\n    \"username\": \"Test\",\r\n    \"password\": \"HashedString\",\r\n    \"scopes\":[\r\n        \"User\"\r\n    ]\r\n}";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.UpdateUser(It.IsAny<UserModel>())).ReturnsAsync(response);

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            bool updated = await _apiService.UpdateUser(new());

            Assert.IsTrue(updated);
        }
        
        /// <summary>
        /// Checks whether the GetAlerts method works as expected.
        /// </summary>
        [TestMethod]
        public async Task TestGetAlerts()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "{\r\n    \"entries\": [\r\n        {\r\n            \"alertId\": 1,\r\n            \"reporter\": \"UnitTester\",\r\n            \"component\": \"component\",\r\n            \"componentStatus\": \"Offline\",\r\n            \"alertStatus\": \"Reported\",\r\n            \"alertDate\": \"2025-06-14T15:39:21.337Z\",\r\n            \"server\": {\r\n                \"hostName\": \"LocalHost\",\r\n                \"game\": \"Minecraft\",\r\n                \"gameVersion\": \"1.7.10\"\r\n            }\r\n        }\r\n    ],\r\n    \"entryCount\": 1,\r\n    \"pageNumber\": 1,\r\n    \"pageSize\": 25,\r\n    \"totalPageCount\": 1,\r\n    \"totalCount\": 1\r\n}";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetAlerts(It.IsAny<int>())).ReturnsAsync(response);

            APIAlertsModel expected = new()
            {
                Alerts =
                [
                    new()
                    {
                        Id = 1,
                        Occured = new(2025, 06, 14, 15, 39, 21, DateTimeKind.Utc),
                        Server = "Minecraft (1.7.10)",
                        Reporter = "UnitTester",
                        Component = "component",
                        ComponentStatus = "Offline",
                        AlertStatus = "Reported"
                    }
                ],
                MultiplePages = false,
                PageCount = 1,
                APICalled = true
            };

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            APIAlertsModel actual = await _apiService.GetAlerts(1);

            Assert.AreEqual(expected.Alerts[0].Reporter, actual.Alerts[0].Reporter);
            Assert.AreEqual(expected.Alerts[0].Server, actual.Alerts[0].Server);
            Assert.AreEqual(expected.Alerts[0].Occured, actual.Alerts[0].Occured);
            Assert.AreEqual(expected.Alerts[0].Component, actual.Alerts[0].Component);
            Assert.AreEqual(expected.Alerts[0].ComponentStatus, actual.Alerts[0].ComponentStatus);
            Assert.AreEqual(expected.Alerts[0].AlertStatus, actual.Alerts[0].AlertStatus);
        }
        
        // Checks whether the GetAlert method works as expected.
        [TestMethod]
        public void TestGetAlert()
        {
            DateTime utcNow = new(2026, 03, 12, 16, 00, 00, DateTimeKind.Utc);
            DateTime expiryDate = utcNow.AddMinutes(15);
            string responseContent = "{\r\n    \"entries\": [\r\n        {\r\n            \"alertId\": 1,\r\n            \"reporter\": \"UnitTester\",\r\n            \"component\": \"component\",\r\n            \"componentStatus\": \"Offline\",\r\n            \"alertStatus\": \"Reported\",\r\n            \"alertDate\": \"2025-06-14T15:39:21.337Z\",\r\n            \"server\": {\r\n                \"hostName\": \"LocalHost\",\r\n                \"game\": \"Minecraft\",\r\n                \"gameVersion\": \"1.7.10\"\r\n            }\r\n        }\r\n    ],\r\n    \"entryCount\": 1,\r\n    \"pageNumber\": 1,\r\n    \"pageSize\": 25,\r\n    \"totalPageCount\": 1,\r\n    \"totalCount\": 1\r\n}";
            RestResponse response = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            };

            Mock<ILoggerService> _mockLogger = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(c => c.UtcNow).Returns(utcNow);
            Mock<IAPIClient> _mockAPIClient = new();
            _mockAPIClient.Setup(api => api.GetAlerts(It.IsAny<int>())).ReturnsAsync(response);

            APIAlertsModel expected = new()
            {
                Alerts =
                [
                    new()
                    {
                        Id = 1,
                        Occured = new(2025, 06, 14, 15, 39, 21, DateTimeKind.Utc),
                        Server = "Minecraft (1.7.10)",
                        Reporter = "UnitTester",
                        Component = "component",
                        ComponentStatus = "Offline",
                        AlertStatus = "Reported"
                    }
                ],
                MultiplePages = false,
                PageCount = 1,
                APICalled = true
            };

            APIService _apiService = new(_mockLogger.Object, _mockAPIClient.Object, _mockClock.Object)
            {
                ExpiryTime = expiryDate
            };

            APIAlertsModel actual = await _apiService.GetAlerts(1);

            Assert.AreEqual(expected.Alerts[0].Reporter, actual.Alerts[0].Reporter);
            Assert.AreEqual(expected.Alerts[0].Server, actual.Alerts[0].Server);
            Assert.AreEqual(expected.Alerts[0].Occured, actual.Alerts[0].Occured);
            Assert.AreEqual(expected.Alerts[0].Component, actual.Alerts[0].Component);
            Assert.AreEqual(expected.Alerts[0].ComponentStatus, actual.Alerts[0].ComponentStatus);
            Assert.AreEqual(expected.Alerts[0].AlertStatus, actual.Alerts[0].AlertStatus);
        }
        /*
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

        // Checks whether the RegisterAlert method works as expected.
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
