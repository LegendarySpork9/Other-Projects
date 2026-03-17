// Copyright © - Unpublished - Toby Hunter
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Converters;
using ServerStatusCommon.Models;
using ServerStatusCommon.Models.Requests;
using ServerStatusCommon.Models.Responses;
using ServerStatusCommon.Models.Responses.Related;

namespace ServerStatusCommon.Implementations
{
    public class APIClientWrapper : IAPIClient
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly SharedSettingsModel SharedSettings;

        private string? BearerToken;

        // Sets the class's global variables.
        public APIClientWrapper(
            ILoggerService _logger,
            IFileSystem _fileSystem,
            SharedSettingsModel sharedSettings)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            SharedSettings = sharedSettings;
        }

        /// <summary>
        /// Returns the token expiry time from the API.
        /// </summary>
        public async Task<(DateTime?, bool)> Authorise()
        {
            DateTime? expiryTime = null;
            bool success = false;

            try
            {
                string url = BuildURL("/auth/token");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\Authorise.json");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    AuthenticationModel? auth = JsonConvert.DeserializeObject<AuthenticationModel>(response.Content) ?? null;

                    if (auth != null)
                    {
                        BearerToken = auth.Token;

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {BearerToken}");

                        expiryTime = DateTime.SpecifyKind(auth.Expires, DateTimeKind.Utc);

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {expiryTime}");

                        success = true;
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (expiryTime, success);
        }

        /// <summary>
        /// Returns a list of users from the API.
        /// </summary>
        public async Task<(List<UserModel>, bool)> GetUsers()
        {
            List<UserModel> users = [];
            bool success = false;

            try
            {
                string url = BuildURL("/users");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    users = JsonConvert.DeserializeObject<List<UserModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {users.Count}");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (users, success);
        }

        /// <summary>
        /// Returns a list of user settings from the API for a given user.
        /// </summary>
        public async Task<(List<UserSettingsModel>, bool)> GetUserSettings(int userId)
        {
            List<UserSettingsModel> userSettings = [];
            bool success = false;

            try
            {
                string url = BuildURL("/usersettings", userId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    userSettings = JsonConvert.DeserializeObject<List<UserSettingsModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {userSettings.Sum(us => us.Settings.Count)}");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (userSettings, success);
        }

        /// <summary>
        /// Returns a list of servers from the API.
        /// </summary>
        public async Task<(List<ServerModel>, bool)> GetServers()
        {
            List<ServerModel> servers = [];
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serverinformation");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    servers = JsonConvert.DeserializeObject<List<ServerModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Servers Returned: {servers.Count}");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (servers, success);
        }

        /// <summary>
        /// Returns a list of server status from the API for a given component.
        /// </summary>
        public async Task<(List<ServerEventModel>, bool)> GetServerStatuses(string component)
        {
            List<ServerEventModel> serverEvents = [];
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serverevent", null, [new("{component}", component)]);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    serverEvents = JsonConvert.DeserializeObject<List<ServerEventModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Events Returned: {serverEvents.Count}");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (serverEvents, success);
        }

        /// <summary>
        /// Updates the given user setting in the API.
        /// </summary>
        public async Task<bool> UpdateUserSettings(int userSettingsId, string value)
        {
            bool success = false;

            try
            {
                string url = BuildURL("/usersettings", userSettingsId, null, true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = new()
                {
                    ["value"] = value
                };

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Updated User Setting: True");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Updates the given user in the API.
        /// </summary>
        public async Task<bool> UpdateUser(UserModel user)
        {
            bool success = false;

            try
            {
                string url = BuildURL("/user", user.Id, null, true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = new()
                {
                    ["username"] = user.Username,
                    ["password"] = user.Password,
                    ["scopes"] = JArray.FromObject(user.Scopes)
                };

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Updated User: True");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Returns a list of alerts from the API.
        /// </summary>
        public async Task<(AlertInformationModel?, bool)> GetAlerts(int pageNumber)
        {
            AlertInformationModel? alerts = null;
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serveralert");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };
                request.AddParameter("page", pageNumber);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    alerts = JsonConvert.DeserializeObject<AlertInformationModel>(response.Content);

                    if (alerts != null)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alerts Returned: {alerts.EntryCount}");

                        success = true;
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (alerts, success);
        }

        /// <summary>
        /// Returns a given alert from the API.
        /// </summary>
        public async Task<(AlertModel?, bool)> GetAlert(int alertId)
        {
            AlertModel? alert = null;
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serveralert", alertId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    alert = JsonConvert.DeserializeObject<AlertModel>(response.Content);

                    if (alert != null)
                    {
                        success = true;
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (alert, success);
        }

        /// <summary>
        /// Updates the given alert in the API.
        /// </summary>
        public async Task<bool> UpdateAlert(int alertId, string status)
        {
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serveralert", alertId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = new()
                {
                    ["status"] = status
                };

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Updated Alert: True");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Adds the given alert to the API.
        /// </summary>
        public async Task<bool> RegisterAlert(NewAlertModel alert)
        {
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serveralert");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(alert);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Registered Alert: True");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Adds the given event to the API.
        /// </summary>
        public async Task<bool> RegisterServerEvent(NewEventModel newEvent)
        {
            bool success = false;

            try
            {
                string url = BuildURL("/serverstatus/serverevent");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(newEvent);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Registered Event: True");

                    success = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Returns the API url.
        /// </summary>
        private string BuildURL(string endpoint, object? entityId = null, List<KeyValuePair<string, object>>? queryParameters = null, bool ignoreQuery = false)
        {
            string url = $"{SharedSettings.BaseURL}{endpoint}";
            string query = APIConverter.GetQuery(endpoint);

            if (entityId != null)
            {
                url += $"/{entityId}";
            }

            if (queryParameters != null)
            {
                foreach(KeyValuePair<string, object> queryParameter in queryParameters)
                {
                    query = query.Replace($"{queryParameter.Key}", $"{queryParameter.Value}");
                }
            }

            if (!ignoreQuery)
            {
                url += query;
            }

            return url;
        }
    }
}
