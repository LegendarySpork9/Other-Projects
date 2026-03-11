// Copyright © - 05/10/2025 - Toby Hunter
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Converters;
using ServerStatusCommon.Models;
using ServerStatusCommon.Models.API;
using ServerStatusCommon.Models.Data;

namespace ServerStatusCommon.Services
{
    public class APIService
    {
        private ILoggerService _Logger;
        private readonly IAPIClient _APIClient;
        private readonly IClock _Clock;
        
        public DateTime ExpiryTime { get; set; }
        private int RetryCount { get; set; } = 0;

        // Sets the class's global variables.
        public APIService(
            ILoggerService _logger,
            IAPIClient _apiClient,
            IClock _clock)
        {
            _Logger = _logger;
            _APIClient = _apiClient;
            _Clock = _clock;
        }

        /// <summary>
        /// Sets the logger.
        /// </summary>
        public void SetLogger(ILoggerService _logger)
        {
            _Logger = _logger;
        }

        /// <summary>
        /// Gets a bearer token from the API.
        /// </summary>
        public async Task Authorise()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                DateTime? expiryTime = await _APIClient.Authorise();

                if (expiryTime.HasValue)
                {
                    ExpiryTime = expiryTime.Value;
                }

                else
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await _APIClient.Authorise();
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
        }

        /// <summary>
        /// Gets a list of the users from the API.
        /// </summary>
        public async Task<List<UserModel>> GetUsers()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<UserModel> users = [];

            try
            {
                RestResponse? response = await _APIClient.GetUsers();

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {responseContent.Count}");

                    foreach (JObject user in responseContent)
                    {
                        users.Add(new UserModel()
                        {
                            UserId = int.Parse(user.Property("id")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                            Username = user.Property("username")?.Value.ToString() ?? StandardValues.MissingValues.Username,
                            Password = user.Property("password")?.Value.ToString() ?? StandardValues.MissingValues.Password
                        });

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Id: {users[^1].UserId}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Username: {users[^1].Username}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Password: {users[^1].Password}");
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched users from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        users = await GetUsers();
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
            }

            RetryCount = 0;
            return users;
        }

        /// <summary>
        /// Gets the user settings for the given user.
        /// </summary>
        public async Task<UserModel> GetUserSettings(UserModel user)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching user settings from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            try
            {
                RestResponse? response = await _APIClient.GetUserSettings(user.UserId);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name")?.Value.ToString() == "DiscordName")
                        {
                            user.DiscordName = setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingStringValue;

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Discord: {user.DiscordName}");
                        }

                        if (setting.Property("name")?.Value.ToString() == "IsAdmin")
                        {
                            user.Admin = bool.Parse(setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingBoolValue);

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Admin: {user.Admin}");
                        }

                        if (setting.Property("name")?.Value.ToString() == "DarkMode")
                        {
                            user.DarkMode = bool.Parse(setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingBoolValue);

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Dark Mode: {user.DarkMode}");
                        }
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user settings from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        user = await GetUserSettings(user);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user settings from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user settings from API");
            }

            RetryCount = 0;
            return user;
        }

        /// <summary>
        /// Gets the UserSettingID for a given user & setting.
        /// </summary>
        public async Task<int> GetUserSettingId(int userId, string settingName)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching user settings from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            int userSettingId = 0;

            try
            {
                RestResponse? response = await _APIClient.GetUserSettings(userId);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name")?.Value.ToString() == settingName)
                        {
                            userSettingId = int.Parse(setting.Property("id")?.Value.ToString() ?? StandardValues.MissingValues.Integer);

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Setting Id: {userSettingId}");
                        }
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user setting id from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        userSettingId = await GetUserSettingId(userId, settingName);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user setting id from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user setting id from API");
            }

            RetryCount = 0;
            return userSettingId;
        }

        /// <summary>
        /// Gets a list of active servers.
        /// </summary>
        public async Task<List<ServerModel>> GetServers()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching servers from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<ServerModel> servers = [new()];

            try
            {
                RestResponse? response = await _APIClient.GetServers();

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content) && !response.Content.Contains("No data returned by given parameters."))
                    {
                        JArray responseContent = JArray.Parse(response.Content);

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Servers Returned: {responseContent.Count}");

                        List<APIStatusModel> pcStatuses = await GetServerStatuses("PC Status");
                        List<APIStatusModel> serverStatuses = await GetServerStatuses("Server Status");
                        List<APIStatusModel> connectionStatuses = await GetServerStatuses("Connection Status");

                        foreach (JObject server in responseContent)
                        {
                            string hostName = server.Property("hostName")?.Value.ToString() ?? StandardValues.MissingValues.HostName;
                            string game = server.Property("game")?.Value.ToString() ?? StandardValues.MissingValues.Game;
                            string gameVersion = server.Property("gameVersion")?.Value.ToString() ?? StandardValues.MissingValues.GameVersion;

                            JObject connection = JObject.Parse(server.Property("connection")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                            string ipAddress = connection.Property("ipAddress")?.Value.ToString() ?? StandardValues.MissingValues.IpAddress;
                            int port = int.Parse(connection.Property("port")?.Value.ToString() ?? StandardValues.MissingValues.Port);

                            JObject downtime = JObject.Parse(server.Property("downtime")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                            string? time = null;

                            if (downtime.ToString() != StandardValues.MissingValues.RelatedContent)
                            {
                                time = downtime.Property("time")?.Value.ToString();
                            }

                            APIStatusModel? pcStatus = pcStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                            APIStatusModel? serverStatus = serverStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                            APIStatusModel? connectionStatus = connectionStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);

                            if (pcStatus != null && serverStatus != null && connectionStatus != null)
                            {
                                DowntimeModel? dt = null;

                                if (!string.IsNullOrWhiteSpace(time))
                                {
                                    dt = new DowntimeModel()
                                    {
                                        Time = time
                                    };
                                }

                                List<StatusModel> statuses =
                                [
                                    new StatusModel()
                                    {
                                        Status = pcStatus.Status,
                                        StatusClass = APIConverter.GetStatusClass(pcStatus.Status)
                                    },
                                    new StatusModel()
                                    {
                                        Status = serverStatus.Status,
                                        StatusClass = APIConverter.GetStatusClass(serverStatus.Status)
                                    },
                                    new StatusModel()
                                    {
                                        Status = connectionStatus.Status,
                                        StatusClass = APIConverter.GetStatusClass(connectionStatus.Status)
                                    }
                                ];

                                servers.Add(new ServerModel()
                                {
                                    HostName = hostName,
                                    Game = game,
                                    GameVersion = gameVersion,
                                    Connection = new ConnectionModel()
                                    {
                                        IPAddress = ipAddress,
                                        Port = port
                                    },
                                    Downtime = dt,
                                    Statuses = statuses
                                });

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {hostName}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {game}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {gameVersion}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {ipAddress}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Downtime: {time}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status: {statuses[0].Status}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status Class: {statuses[0].StatusClass}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status: {statuses[1].Status}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status Class: {statuses[1].StatusClass}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Connection Status: {statuses[2].Status}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Connection Status Class: {statuses[2].StatusClass}");
                            }
                        }
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched servers from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        servers = await GetServers();
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch servers from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch servers from API");
            }

            RetryCount = 0;
            return servers;
        }

        /// <summary>
        /// Gets a ist of the statuses for a given component.
        /// </summary>
        public async Task<List<APIStatusModel>> GetServerStatuses(string component)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching server statuses from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<APIStatusModel> statuses = [];

            try
            {
                RestResponse? response = await _APIClient.GetServerStatuses(component);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Statuses Returned: {responseContent.Count}");

                    foreach (JObject status in responseContent)
                    {
                        JObject server = JObject.Parse(status.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                        statuses.Add(new APIStatusModel()
                        {
                            Component = status.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                            Status = status.Property("status")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                            DateOccured = DateTime.SpecifyKind(DateTime.Parse(status.Property("dateOccured")?.Value.ToString()), DateTimeKind.Utc),
                            Server = new APIRelatedServerModel()
                            {
                                HostName = server.Property("hostName")?.Value.ToString() ?? StandardValues.MissingValues.HostName,
                                Game = server.Property("game")?.Value.ToString() ?? StandardValues.MissingValues.Game,
                                GameVersion = server.Property("gameVersion")?.Value.ToString() ?? StandardValues.MissingValues.GameVersion
                            }
                        });

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {statuses[^1].Component}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {statuses[^1].Status}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {statuses[^1].DateOccured}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {statuses[^1].Server.HostName}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {statuses[^1].Server.Game}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {statuses[^1].Server.GameVersion}");
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched server statuses from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        statuses = await GetServerStatuses(component);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch server statuses from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch server statuses from API");
            }

            RetryCount = 0;
            return statuses;
        }

        /// <summary>
        /// Changes the value of the given user setting.
        /// </summary>
        public async Task<bool> UpdateUserSettings(int userSettingsId, string value)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user setting in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool updated = false;

            try
            {
                RestResponse? response = await _APIClient.UpdateUserSettings(userSettingsId, value);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user setting in API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        updated = await UpdateUserSettings(userSettingsId, value);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update setting in API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update setting in API");
            }

            RetryCount = 0;
            return updated;
        }

        /// <summary>
        /// Changes the information of a given user.
        /// </summary>
        public async Task<bool> UpdateUser(UserModel user)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user details in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool updated = false;

            try
            {
                RestResponse? response = await _APIClient.UpdateUser(user);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user details in API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        updated = await UpdateUser(user);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update user details in API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update user details in API");
            }

            RetryCount = 0;
            return updated;
        }

        /// <summary>
        /// Gets the alerts on a given page.
        /// </summary>
        public async Task<APIAlertsModel> GetAlerts(int pageNumber)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alerts from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            APIAlertsModel alerts = new();

            try
            {
                RestResponse? response = await _APIClient.GetAlerts(pageNumber);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content) && !response.Content.Contains("No data returned by given parameters."))
                    {
                        JObject responseContent = JObject.Parse(response.Content);
                        JArray alertsContent = JArray.Parse(responseContent.Property("entries")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alerts Returned: {alertsContent.Count}");

                        if (alertsContent.Count > 0)
                        {
                            foreach (JObject alert in alertsContent)
                            {
                                JObject server = JObject.Parse(alert.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                                alerts.Alerts.Add(new AlertModel
                                {
                                    Id = int.Parse(alert.Property("alertId")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                                    Occured = DateTime.SpecifyKind(DateTime.Parse(alert.Property("alertDate")?.Value.ToString()), DateTimeKind.Utc),
                                    Server = $"{server.Property("game")?.Value ?? StandardValues.MissingValues.Game} ({server.Property("gameVersion")?.Value ?? StandardValues.MissingValues.GameVersion})",
                                    Reporter = alert.Property("reporter")?.Value.ToString() ?? StandardValues.MissingValues.Reporter,
                                    Component = alert.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                                    ComponentStatus = alert.Property("componentStatus")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                                    AlertStatus = alert.Property("alertStatus")?.Value.ToString() ?? StandardValues.MissingValues.AlertStatus
                                });

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alerts.Alerts[^1].Id}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alerts.Alerts[^1].Occured}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {alerts.Alerts[^1].Server}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alerts.Alerts[^1].Reporter}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alerts.Alerts[^1].Component}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alerts.Alerts[^1].ComponentStatus}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alerts.Alerts[^1].AlertStatus}");
                            }

                            if (int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer) > 1)
                            {
                                alerts.MultiplePages = true;
                                alerts.PageCount = int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer);

                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Multiple Pages: {alerts.MultiplePages}");
                                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Count: {alerts.PageCount}");
                            }
                        }
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alerts from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        alerts = await GetAlerts(pageNumber);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
            }

            alerts.APICalled = true;

            RetryCount = 0;
            return alerts;
        }

        /// <summary>
        /// Gets the information of a given AlertID.
        /// </summary>
        public async Task<AlertModel> GetAlert(int alertId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alert from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            AlertModel alert = new();

            try
            {
                RestResponse? response = await _APIClient.GetAlert(alertId);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseContent = JObject.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JObject server = JObject.Parse(responseContent.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    alert = new AlertModel
                    {
                        Id = int.Parse(responseContent.Property("alertId")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                        Occured = DateTime.SpecifyKind(DateTime.Parse(responseContent.Property("alertDate")?.Value.ToString()), DateTimeKind.Utc),
                        Server = $"{server.Property("game")?.Value ?? StandardValues.MissingValues.Game} ({server.Property("gameVersion")?.Value ?? StandardValues.MissingValues.GameVersion})",
                        Reporter = responseContent.Property("reporter")?.Value.ToString() ?? StandardValues.MissingValues.Reporter,
                        Component = responseContent.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                        ComponentStatus = responseContent.Property("componentStatus")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                        AlertStatus = responseContent.Property("alertStatus")?.Value.ToString() ?? StandardValues.MissingValues.AlertStatus
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alert.Id}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alert.Occured}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {alert.Server}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alert.Reporter}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alert.Component}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alert.ComponentStatus}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alert.AlertStatus}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alert from API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        alert = await GetAlert(alertId);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alert from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alert from API");
            }

            RetryCount = 0;
            return alert;
        }

        /// <summary>
        /// Changes the status of the given alert.
        /// </summary>
        public async Task<bool> UpdateAlert(int alertId, string status)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating alert status in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool updated = false;

            try
            {
                RestResponse? response = await _APIClient.UpdateAlert(alertId, status);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Status Updated");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated alert status in API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        updated = await UpdateAlert(alertId, status);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update alert status in API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update alert status in API");
            }

            RetryCount = 0;
            return updated;
        }

        /// <summary>
        /// Adds a new alert to the API.
        /// </summary>
        public async Task<bool> RegisterAlert(APINewAlertsModel alert)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering alert in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool registered = false;

            try
            {
                RestResponse? response = await _APIClient.RegisterAlert(alert);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    registered = true;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Register Successful");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered alert in API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        registered = await RegisterAlert(alert);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server alert in API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register alert in API");
            }

            RetryCount = 0;
            return registered;
        }

        /// <summary>
        /// Adds a new event to the API.
        /// </summary>
        public async Task<bool> RegisterServerEvent(APIStatusModel status)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering server event in API");

            if (ExpiryTime <= _Clock.UtcNow)
            {
                await Authorise();
            }

            bool registered = false;

            try
            {
                RestResponse? response = await _APIClient.RegisterServerEvent(status);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    registered = true;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Register Successful");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered server event in API");
                }

                else if (response == null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == 0)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        registered = await RegisterServerEvent(status);
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server event in API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server event in API");
            }

            RetryCount = 0;
            return registered;
        }
    }
}
