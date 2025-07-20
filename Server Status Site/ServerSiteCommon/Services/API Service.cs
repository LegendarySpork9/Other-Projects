using Newtonsoft.Json.Linq;
using RestSharp;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;

namespace ServerSiteCommon.Services
{
    public class APIService
    {
        private LoggerService Logger { get; set; }
        private SharedSettingsModel SharedSettings { get; set; }
        private string[] Endpoints { get; set; }
        private string BearerToken { get; set; }
        public DateTime ExpiryTime { get; set; }
        private int RetryCount { get; set; } = 0;

        // Sets the class's global variables.
        public APIService(SharedSettingsModel sharedSettings)
        {
            SharedSettings = sharedSettings;
            Endpoints = sharedSettings.Endpoints.Split(',');
        }

        // Sets the logger.
        public void SetLogger(LoggerService _loggerService)
        {
            Logger = _loggerService;
        }

        // Gets a bearer token from the API.
        public void Authorise()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Authorisation:"))?.Replace("Authorisation:", "") ?? StandardValues.MissingValues.AuthEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", File.ReadAllText($@"{SharedSettings.PayloadLocation}\Authorise.json"), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {File.ReadAllText($@"{SharedSettings.PayloadLocation}\Authorise.json")}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseContent = JObject.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    BearerToken = responseContent.Property("token")?.Value.ToString() ?? StandardValues.MissingValues.BearerToken;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {BearerToken}");

                    JObject infoContent = JObject.Parse(responseContent.Property("info")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);
                    ExpiryTime = DateTime.Parse(infoContent.Property("expires")?.Value.ToString() ?? StandardValues.MissingValues.DT);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {ExpiryTime}");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
        }

        // Gets a bearer token from the API.
        public async Task AuthoriseAsync()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Authorisation:"))?.Replace("Authorisation:", "") ?? StandardValues.MissingValues.AuthEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", File.ReadAllText($@"{SharedSettings.PayloadLocation}\Authorise.json"), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {File.ReadAllText($@"{SharedSettings.PayloadLocation}\Authorise.json")}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseContent = JObject.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    BearerToken = responseContent.Property("token")?.Value.ToString() ?? StandardValues.MissingValues.BearerToken;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {BearerToken}");

                    JObject infoContent = JObject.Parse(responseContent.Property("info")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);
                    ExpiryTime = DateTime.Parse(infoContent.Property("expires")?.Value.ToString() ?? StandardValues.MissingValues.DT);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {ExpiryTime}");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
        }

        // Gets a list of the users from the API.
        public async Task<List<UserModel>> GetUsers()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            List<UserModel> users = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Users:"))?.Replace("Users:", "") ?? StandardValues.MissingValues.UserEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {responseContent.Count}");

                    foreach (JObject user in responseContent)
                    {
                        users.Add(new UserModel()
                        {
                            UserId = int.Parse(user.Property("id")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                            Username = user.Property("username")?.Value.ToString() ?? StandardValues.MissingValues.Username,
                            Password = user.Property("password")?.Value.ToString() ?? StandardValues.MissingValues.Password
                        });

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Id: {users[^1].UserId}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Username: {users[^1].Username}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Password: {users[^1].Password}");
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched users from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        users = await GetUsers();
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
            }

            RetryCount = 0;
            return users;
        }

        // Gets the user settings for the given user.
        public async Task<UserModel> GetUserSettings(UserModel user)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching user settings from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:"))?.Replace("Settings:", "") ?? StandardValues.MissingValues.SettingsEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{user.UserId}?application=Server Status Site";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name")?.Value.ToString() == "DiscordName")
                        {
                            user.DiscordName = setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingStringValue;

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Discord: {user.DiscordName}");
                        }

                        if (setting.Property("name")?.Value.ToString() == "IsAdmin")
                        {
                            user.Admin = bool.Parse(setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingBoolValue);

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Admin: {user.Admin}");
                        }

                        if (setting.Property("name")?.Value.ToString() == "DarkMode")
                        {
                            user.DarkMode = bool.Parse(setting.Property("value")?.Value.ToString() ?? StandardValues.MissingValues.SettingBoolValue);

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Dark Mode: {user.DarkMode}");
                        }
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user settings from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        user = await GetUserSettings(user);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user settings from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user settings from API");
            }

            RetryCount = 0;
            return user;
        }

        // Gets the UserSettingID for a given user & setting.
        public int GetUserSettingId(int userId, string settingName)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching user settings from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            int userSettingId = 0;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:"))?.Replace("Settings:", "") ?? StandardValues.MissingValues.SettingsEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{userId}?application=Server Status Site";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name")?.Value.ToString() == settingName)
                        {
                            userSettingId = int.Parse(setting.Property("id")?.Value.ToString() ?? StandardValues.MissingValues.Integer);

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Setting Id: {userSettingId}");
                        }
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user setting id from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        userSettingId = GetUserSettingId(userId, settingName);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user setting id from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch user setting id from API");
            }

            RetryCount = 0;
            return userSettingId;
        }

        // Gets a list of active servers.
        public List<ServerModel> GetServers()
        {
            APIConverter _apiConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching servers from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            List<ServerModel> servers = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Servers:"))?.Replace("Servers:", "") ?? StandardValues.MissingValues.ServerEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + "?IsActive=true";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content) && !response.Content.Contains("No data returned by given parameters."))
                    {
                        JArray responseContent = JArray.Parse(response.Content);

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Servers Returned: {responseContent.Count}");

                        List<APIStatusModel> pcStatuses = GetServerStatuses("PC Status");
                        List<APIStatusModel> hamachiStatuses = GetServerStatuses("Hamachi Status");
                        List<APIStatusModel> serverStatuses = GetServerStatuses("Server Status");

                        foreach (JObject server in responseContent)
                        {
                            string hostName = server.Property("hostName")?.Value.ToString() ?? StandardValues.MissingValues.HostName;
                            string game = server.Property("game")?.Value.ToString() ?? StandardValues.MissingValues.Game;
                            string gameVersion = server.Property("gameVersion")?.Value.ToString() ?? StandardValues.MissingValues.GameVersion;
                            string ipAddress = server.Property("ipAddress")?.Value.ToString() ?? StandardValues.MissingValues.IpAddress;

                            APIStatusModel? pcStatus = pcStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                            APIStatusModel? hamachiStatus = hamachiStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                            APIStatusModel? serverStatus = serverStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);

                            if (pcStatus != null && hamachiStatus != null && serverStatus != null)
                            {
                                List<StatusModel> statuses = new()
                                {
                                    new StatusModel()
                                    {
                                        Status = pcStatus.Status,
                                        StatusClass = _apiConverter.GetStatusClass(pcStatus.Status)
                                    },
                                    new StatusModel()
                                    {
                                        Status = hamachiStatus.Status,
                                        StatusClass = _apiConverter.GetStatusClass(hamachiStatus.Status)
                                    },
                                    new StatusModel()
                                    {
                                        Status = serverStatus.Status,
                                        StatusClass = _apiConverter.GetStatusClass(serverStatus.Status)
                                    }
                                };

                                servers.Add(new ServerModel()
                                {
                                    HostName = hostName,
                                    Game = game,
                                    GameVersion = gameVersion,
                                    IPAddress = ipAddress,
                                    Statuses = statuses
                                });

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {hostName}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {game}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {gameVersion}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {ipAddress}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status: {statuses[0].Status}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status Class: {statuses[0].StatusClass}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hamachi Status: {statuses[1].Status}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hamachi Status Class: {statuses[1].StatusClass}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status: {statuses[2].Status}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status Class: {statuses[2].StatusClass}");
                            }
                        }
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched servers from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        servers = GetServers();
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch servers from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch servers from API");
            }

            RetryCount = 0;
            return servers;
        }

        // Gets a ist of the statuses for a given component.
        public List<APIStatusModel> GetServerStatuses(string component)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching server statuses from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            List<APIStatusModel> statuses = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Statuses:"))?.Replace("Statuses:", "") ?? StandardValues.MissingValues.StatusEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + $"?Component={component}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JArray responseContent = JArray.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Statuses Returned: {responseContent.Count}");

                    foreach (JObject status in responseContent)
                    {
                        JObject server = JObject.Parse(status.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                        statuses.Add(new APIStatusModel()
                        {
                            Component = status.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                            Status = status.Property("status")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                            DateOccured = DateTime.Parse(status.Property("dateOccured")?.Value.ToString() ?? StandardValues.MissingValues.DT),
                            Server = new APIRelatedServerModel()
                            {
                                HostName = server.Property("hostName")?.Value.ToString() ?? StandardValues.MissingValues.HostName,
                                Game = server.Property("game")?.Value.ToString() ?? StandardValues.MissingValues.Game,
                                GameVersion = server.Property("gameVersion")?.Value.ToString() ?? StandardValues.MissingValues.GameVersion
                            }
                        });

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {statuses[^1].Component}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {statuses[^1].Status}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {statuses[^1].DateOccured}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {statuses[^1].Server.HostName}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {statuses[^1].Server.Game}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {statuses[^1].Server.GameVersion}");
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched server statuses from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        statuses = GetServerStatuses(component);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch server statuses from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch server statuses from API");
            }

            RetryCount = 0;
            return statuses;
        }

        // Changes the value of the given user setting.
        public async Task<bool> UpdateUserSettings(int userSettingsId, string value)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user setting in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:"))?.Replace("Settings:", "") ?? StandardValues.MissingValues.SettingsEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{userSettingsId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateUserSettings.json"));
                json.Property("value").Value = value;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user setting in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        updated = await UpdateUserSettings(userSettingsId, value);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update setting in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update setting in API");
            }

            RetryCount = 0;
            return updated;
        }

        // Changes the information of a given user.
        public async Task<bool> UpdateUser(UserModel user)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user details in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Users:"))?.Replace("Users:", "") ?? StandardValues.MissingValues.UserEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{user.UserId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateUser.json"));
                json.Property("Username").Value = user.Username;
                json.Property("Password").Value = user.Password;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user details in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        updated = await UpdateUser(user);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update user details in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update user details in API");
            }

            RetryCount = 0;
            return updated;
        }

        // Gets the alerts on a given page.
        public APIAlertsModel GetAlerts(int pageNumber)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alerts from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            APIAlertsModel alerts = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + $"?PageNumber={pageNumber}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content) && !response.Content.Contains("No data returned by given parameters."))
                    {
                        JObject responseContent = JObject.Parse(response.Content);
                        JArray alertsContent = JArray.Parse(responseContent.Property("entries")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alerts Returned: {alertsContent.Count}");

                        if (alertsContent.Count > 0)
                        {
                            foreach (JObject alert in alertsContent)
                            {
                                JObject server = JObject.Parse(alert.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                                alerts.Alerts.Add(new AlertModel
                                {
                                    Id = int.Parse(alert.Property("alertId")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                                    Occured = DateTime.Parse(alert.Property("alertDate")?.Value.ToString() ?? StandardValues.MissingValues.DT),
                                    Server = $"{server.Property("game")?.Value ?? StandardValues.MissingValues.Game} ({server.Property("gameVersion")?.Value ?? StandardValues.MissingValues.GameVersion})",
                                    Reporter = alert.Property("reporter")?.Value.ToString() ?? StandardValues.MissingValues.Reporter,
                                    Component = alert.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                                    ComponentStatus = alert.Property("componentStatus")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                                    AlertStatus = alert.Property("alertStatus")?.Value.ToString() ?? StandardValues.MissingValues.AlertStatus
                                });

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alerts.Alerts[^1].Id}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alerts.Alerts[^1].Occured}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {alerts.Alerts[^1].Server}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alerts.Alerts[^1].Reporter}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alerts.Alerts[^1].Component}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alerts.Alerts[^1].ComponentStatus}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alerts.Alerts[^1].AlertStatus}");
                            }

                            if (int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer) > 1)
                            {
                                alerts.MultiplePages = true;
                                alerts.PageCount = int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer);

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Multiple Pages: {alerts.MultiplePages}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Count: {alerts.PageCount}");
                            }
                        }
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alerts from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        alerts = GetAlerts(pageNumber);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
            }

            alerts.APICalled = true;

            RetryCount = 0;
            return alerts;
        }

        // Gets the alerts on a given page.
        public async Task<APIAlertsModel> GetAlertsAsync(int pageNumber)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alerts from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            APIAlertsModel alerts = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + $"?PageNumber={pageNumber}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content) && !response.Content.Contains("No data returned by given parameters."))
                    {
                        JObject responseContent = JObject.Parse(response.Content);
                        JArray alertsContent = JArray.Parse(responseContent.Property("entries")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alerts Returned: {alertsContent.Count}");

                        if (alertsContent.Count > 0)
                        {
                            foreach (JObject alert in alertsContent)
                            {
                                JObject server = JObject.Parse(alert.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                                alerts.Alerts.Add(new AlertModel
                                {
                                    Id = int.Parse(alert.Property("alertId")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                                    Occured = DateTime.Parse(alert.Property("alertDate")?.Value.ToString() ?? StandardValues.MissingValues.DT),
                                    Server = $"{server.Property("game")?.Value ?? StandardValues.MissingValues.Game} ({server.Property("gameVersion")?.Value ?? StandardValues.MissingValues.GameVersion})",
                                    Reporter = alert.Property("reporter")?.Value.ToString() ?? StandardValues.MissingValues.Reporter,
                                    Component = alert.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                                    ComponentStatus = alert.Property("componentStatus")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                                    AlertStatus = alert.Property("alertStatus")?.Value.ToString() ?? StandardValues.MissingValues.AlertStatus
                                });

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alerts.Alerts[^1].Id}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alerts.Alerts[^1].Occured}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {alerts.Alerts[^1].Server}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alerts.Alerts[^1].Reporter}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alerts.Alerts[^1].Component}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alerts.Alerts[^1].ComponentStatus}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alerts.Alerts[^1].AlertStatus}");
                            }

                            if (int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer) > 1)
                            {
                                alerts.MultiplePages = true;
                                alerts.PageCount = int.Parse(responseContent.Property("totalPageCount")?.Value.ToString() ?? StandardValues.MissingValues.Integer);

                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Multiple Pages: {alerts.MultiplePages}");
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Count: {alerts.PageCount}");
                            }
                        }
                    }

                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alerts from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        alerts = await GetAlertsAsync(pageNumber);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alerts from API");
            }

            alerts.APICalled = true;

            RetryCount = 0;
            return alerts;
        }

        // Gets the information of a given AlertID.
        public AlertModel GetAlert(int alertId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alert from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            AlertModel alert = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{alertId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseContent = JObject.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    JObject server = JObject.Parse(responseContent.Property("server")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);

                    alert = new AlertModel
                    {
                        Id = int.Parse(responseContent.Property("alertId")?.Value.ToString() ?? StandardValues.MissingValues.Integer),
                        Occured = DateTime.Parse(responseContent.Property("alertDate")?.Value.ToString() ?? StandardValues.MissingValues.DT),
                        Server = $"{server.Property("game")?.Value ?? StandardValues.MissingValues.Game} ({server.Property("gameVersion")?.Value ?? StandardValues.MissingValues.GameVersion})",
                        Reporter = responseContent.Property("reporter")?.Value.ToString() ?? StandardValues.MissingValues.Reporter,
                        Component = responseContent.Property("component")?.Value.ToString() ?? StandardValues.MissingValues.Component,
                        ComponentStatus = responseContent.Property("componentStatus")?.Value.ToString() ?? StandardValues.MissingValues.Status,
                        AlertStatus = responseContent.Property("alertStatus")?.Value.ToString() ?? StandardValues.MissingValues.AlertStatus
                    };

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alert.Id}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alert.Occured}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {alert.Server}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alert.Reporter}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alert.Component}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alert.ComponentStatus}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alert.AlertStatus}");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alert from API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        alert = GetAlert(alertId);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alert from API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch alert from API");
            }

            RetryCount = 0;
            return alert;
        }

        // Changes the status of the given alert.
        public async Task<bool> UpdateAlert(int alertId, string status)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating alert status in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint + @$"/{alertId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateAlert.json"));
                json.Property("status").Value = status;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Status Updated");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated alert status in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        updated = await UpdateAlert(alertId, status);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update alert status in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to update alert status in API");
            }

            RetryCount = 0;
            return updated;
        }

        // Adds a new alert to the API.
        public bool RegisterAlert(APINewAlertsModel alert)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering alert in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            bool registered = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\RegisterAlert.json"));
                json.Property("reporter").Value = alert.Reporter;
                json.Property("component").Value = alert.Component;
                json.Property("componentStatus").Value = alert.ComponentStatus;
                json.Property("alertStatus").Value = alert.AlertStatus;
                json.Property("hostName").Value = alert.HostName;
                json.Property("game").Value = alert.Game;
                json.Property("gameVersion").Value = alert.GameVersion;

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    registered = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Register Successful");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered alert in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        registered = RegisterAlert(alert);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server alert in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register alert in API");
            }

            RetryCount = 0;
            return registered;
        }

        // Adds a new alert to the API.
        public async Task<bool> RegisterAlertAsync(APINewAlertsModel alert)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering alert in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                await AuthoriseAsync();
            }

            bool registered = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:"))?.Replace("Alerts:", "") ?? StandardValues.MissingValues.AlertEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\RegisterAlert.json"));
                json.Property("reporter").Value = alert.Reporter;
                json.Property("component").Value = alert.Component;
                json.Property("componentStatus").Value = alert.ComponentStatus;
                json.Property("alertStatus").Value = alert.AlertStatus;
                json.Property("hostName").Value = alert.HostName;
                json.Property("game").Value = alert.Game;
                json.Property("gameVersion").Value = alert.GameVersion;

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    registered = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Register Successful");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered alert in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await AuthoriseAsync();
                        registered = await RegisterAlertAsync(alert);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server alert in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register alert in API");
            }

            RetryCount = 0;
            return registered;
        }

        // Adds a new event to the API.
        public bool RegisterServerEvent(APIStatusModel status)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering server event in API");

            if (ExpiryTime <= DateTime.UtcNow)
            {
                Authorise();
            }

            bool registered = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Statuses:"))?.Replace("Statuses:", "") ?? StandardValues.MissingValues.StatusEndpoint;
                string url = SharedSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{SharedSettings.PayloadLocation}\RegisterServerEvent.json"));
                json.Property("component").Value = status.Component;
                json.Property("status").Value = status.Status;
                json.Property("hostName").Value = status.Server.HostName;
                json.Property("game").Value = status.Server.Game;
                json.Property("gameVersion").Value = status.Server.GameVersion;

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    registered = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Register Successful");
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered server event in API");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        Authorise();
                        registered = RegisterServerEvent(status);
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server event in API");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register server event in API");
            }

            RetryCount = 0;
            return registered;
        }
    }
}
