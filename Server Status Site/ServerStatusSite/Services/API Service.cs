using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;
using ServerStatusSite.Models.API;
using ServerStatusSite.Models.Data;

namespace ServerStatusSite.Services
{
    public class APIService
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private AppSettingsModel AppSettings { get; set; }
        private string[] Endpoints { get; set; }
        private string BearerToken { get; set; }
        public DateTime ExpiryTime { get; set; }

        public APIService(AppSettingsModel appSettings)
        {
            AppSettings = appSettings;
            Endpoints = AppSettings.Endpoints.Split(',');
        }

        public void SetLogger(LoggerService _loggerService)
        {
            Logger = _loggerService;
        }

        public void Authorise()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Authorisation:")).Replace("Authorisation:", "");
                string url = AppSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", AppSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", File.ReadAllText($@"{AppSettings.PayloadLocation}\Authorise.json"), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {File.ReadAllText($@"{AppSettings.PayloadLocation}\Authorise.json")}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseContent = JObject.Parse(response.Content);
                    BearerToken = responseContent.Property("token").Value.ToString();

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {BearerToken}");

                    JObject infoContent = JObject.Parse(responseContent.Property("info").Value.ToString());
                    ExpiryTime = DateTime.Parse(infoContent.Property("expires").Value.ToString());

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

        public List<UserModel> GetUsers()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            List<UserModel> users = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Users:")).Replace("Users:", "");
                string url = AppSettings.BaseURL + authEndpoint;

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
                    JArray responseContent = JArray.Parse(response.Content);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {responseContent.Count}");

                    foreach (JObject user in responseContent)
                    {
                        users.Add(new UserModel()
                        {
                            UserId = int.Parse(user.Property("id").Value.ToString()),
                            Username = user.Property("username").Value.ToString(),
                            Password = user.Property("password").Value.ToString()
                        });

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Id: {user.Property("id").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Username: {user.Property("username").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Password: {user.Property("password").Value}");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched users from API");
            return users;
        }

        public UserModel GetUserSettings(UserModel user)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching user settings from API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:")).Replace("Settings:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{user.UserId}?application=Server Status Site";

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
                    JArray responseContent = JArray.Parse(response.Content);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings").Value.ToString());

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name").Value.ToString() == "DiscordName")
                        {
                            user.DiscordName = setting.Property("value").Value.ToString();

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Discord: {setting.Property("value").Value}");
                        }

                        if (setting.Property("name").Value.ToString() == "IsAdmin")
                        {
                            user.Admin = bool.Parse(setting.Property("value").Value.ToString());

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Admin: {setting.Property("value").Value}");
                        }

                        if (setting.Property("name").Value.ToString() == "DarkMode")
                        {
                            user.DarkMode = bool.Parse(setting.Property("value").Value.ToString());

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Dark Mode: {setting.Property("value").Value}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user settings from API");
            return user;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:")).Replace("Settings:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{userId}?application=Server Status Site";

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
                    JArray responseContent = JArray.Parse(response.Content);
                    JArray settingsContent = JArray.Parse(JObject.Parse(responseContent[0].ToString()).Property("settings").Value.ToString());

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {settingsContent.Count}");

                    foreach (JObject setting in settingsContent)
                    {
                        if (setting.Property("name").Value.ToString() == settingName)
                        {
                            userSettingId = int.Parse(setting.Property("id").Value.ToString());

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Setting Id: {userSettingId}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched user settings from API");
            return userSettingId;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Servers:")).Replace("Servers:", "");
                string url = AppSettings.BaseURL + authEndpoint + "?IsActive=true";

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
                    JArray responseContent = JArray.Parse(response.Content);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Servers Returned: {responseContent.Count}");

                    List<APIStatusModel> pcStatuses = GetServerStatuses("PC Status");
                    List<APIStatusModel> hamachiStatuses = GetServerStatuses("Hamachi Status");
                    List<APIStatusModel> serverStatuses = GetServerStatuses("Server Status");

                    foreach (JObject server in responseContent)
                    {
                        string hostName = server.Property("hostName").Value.ToString();
                        string game = server.Property("game").Value.ToString();
                        string gameVersion = server.Property("gameVersion").Value.ToString();
                        string ipAddress = server.Property("ipAddress").Value.ToString();

                        APIStatusModel pcStatus = pcStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                        APIStatusModel hamachiStatus = hamachiStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);
                        APIStatusModel serverStatus = serverStatuses.Find(c => c.Server.HostName == hostName && c.Server.Game == game && c.Server.GameVersion == gameVersion);

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
                                IPAddress = server.Property("ipAddress").Value.ToString(),
                                Statuses = statuses
                            });

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {hostName}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {game}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {gameVersion}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {server.Property("ipAddress").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status: {statuses[0].Status}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"PC Status Class: {statuses[0].StatusClass}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hamachi Status: {statuses[1].Status}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hamachi Status Class: {statuses[1].StatusClass}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status: {statuses[2].Status}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Status Class: {statuses[2].StatusClass}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched servers from API");
            return servers;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Statuses:")).Replace("Statuses:", "");
                string url = AppSettings.BaseURL + authEndpoint + $"?Component={component}";

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
                    JArray responseContent = JArray.Parse(response.Content);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Statuses Returned: {responseContent.Count}");

                    foreach (JObject status in responseContent)
                    {
                        JObject server = JObject.Parse(status.Property("server").Value.ToString());

                        statuses.Add(new APIStatusModel()
                        {
                            Component = status.Property("component").Value.ToString(),
                            Status = status.Property("status").Value.ToString(),
                            DateOccured = DateTime.Parse(status.Property("dateOccured").Value.ToString()),
                            Server = new APIRelatedServerModel()
                            {
                                HostName = server.Property("hostName").Value.ToString(),
                                Game = server.Property("game").Value.ToString(),
                                GameVersion = server.Property("gameVersion").Value.ToString()
                            }
                        });

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {status.Property("component").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status: {status.Property("status").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {status.Property("dateOccured").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {server.Property("hostName").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {server.Property("game").Value}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {server.Property("gameVersion").Value}");
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched server statuses from API");
            return statuses;
        }

        public bool UpdateUserSettings(int userSettingsId, string value)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user setting in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Settings:")).Replace("Settings:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{userSettingsId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettings.PayloadLocation}\UpdateUserSettings.json"));
                json.Property("value").Value = value;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user setting in API");
            return updated;
        }

        public bool UpdateUser(UserModel user)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating user details in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Users:")).Replace("Users:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{user.UserId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettings.PayloadLocation}\UpdateUser.json"));
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

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Setting Updated");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated user details in API");
            return updated;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettings.BaseURL + authEndpoint + $"?PageNumber={pageNumber}";

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
                    JObject responseContent = JObject.Parse(response.Content);
                    JArray alertsContent = JArray.Parse(responseContent.Property("entries").Value.ToString());

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alerts Returned: {alertsContent.Count}");

                    if (alertsContent.Count > 0)
                    {
                        foreach (JObject alert in alertsContent)
                        {
                            JObject server = JObject.Parse(alert.Property("server").Value.ToString());

                            alerts.Alerts.Add(new AlertModel
                            {
                                Id = int.Parse(alert.Property("alertId").Value.ToString()),
                                Occured = DateTime.Parse(alert.Property("alertDate").Value.ToString()),
                                Server = $"{server.Property("game").Value} ({server.Property("gameVersion").Value})",
                                Reporter = alert.Property("reporter").Value.ToString(),
                                Component = alert.Property("component").Value.ToString(),
                                ComponentStatus = alert.Property("componentStatus").Value.ToString(),
                                AlertStatus = alert.Property("alertStatus").Value.ToString()
                            });

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {alert.Property("alertId").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {alert.Property("alertDate").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {server.Property("game").Value} ({server.Property("gameVersion").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {alert.Property("reporter").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {alert.Property("component").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {alert.Property("componentStatus").Value}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {alert.Property("alertStatus").Value}");
                        }

                        if (int.Parse(responseContent.Property("totalPageCount").Value.ToString()) > 1)
                        {
                            alerts.MultiplePages = true;
                            alerts.PageCount = int.Parse(responseContent.Property("totalPageCount").Value.ToString());

                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Multiple Pages: {alerts.MultiplePages}");
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Count: {alerts.PageCount}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            alerts.APICalled = true;

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alerts from API");
            return alerts;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{alertId}";

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
                    JObject responseContent = JObject.Parse(response.Content);
                    JObject server = JObject.Parse(responseContent.Property("server").Value.ToString());

                    alert = new AlertModel
                    {
                        Id = int.Parse(responseContent.Property("alertId").Value.ToString()),
                        Occured = DateTime.Parse(responseContent.Property("alertDate").Value.ToString()),
                        Server = $"{server.Property("game").Value} ({server.Property("gameVersion").Value})",
                        Reporter = responseContent.Property("reporter").Value.ToString(),
                        Component = responseContent.Property("component").Value.ToString(),
                        ComponentStatus = responseContent.Property("componentStatus").Value.ToString(),
                        AlertStatus = responseContent.Property("alertStatus").Value.ToString()
                    };

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {responseContent.Property("alertId").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Occured: {responseContent.Property("alertDate").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server: {server.Property("game").Value} ({server.Property("gameVersion").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reporter: {responseContent.Property("reporter").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {responseContent.Property("component").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Status: {responseContent.Property("componentStatus").Value}");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Status: {responseContent.Property("alertStatus").Value}");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched alert from API");
            return alert;
        }

        public bool UpdateAlert(int alertId, string status)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updating alert status in API");

            if (ExpiryTime < DateTime.UtcNow)
            {
                Authorise();
            }

            bool updated = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettings.BaseURL + authEndpoint + @$"/{alertId}";

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettings.PayloadLocation}\UpdateAlert.json"));
                json.Property("status").Value = status;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {json}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = client.Execute(request);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    updated = true;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Status Updated");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Updated alert status in API");
            return updated;
        }

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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettings.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettings.PayloadLocation}\RegisterAlert.json"));
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
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered alert in API");
            return registered;
        }
    }
}
