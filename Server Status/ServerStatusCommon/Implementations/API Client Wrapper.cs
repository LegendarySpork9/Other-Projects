// Copyright © - Unpublished - Toby Hunter
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Converters;
using ServerStatusCommon.Models;
using ServerStatusCommon.Models.API;
using ServerStatusCommon.Models.Data;

namespace ServerStatusCommon.Implementations
{
    public class APIClientWrapper : IAPIClient
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly SharedSettingsModel SharedSettings;

        private string BearerToken;

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
        /// Returns a bearer token from the API.
        /// </summary>
        public async Task<DateTime?> Authorise()
        {
            DateTime? expiryTime = null;

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
                    JObject responseContent = JObject.Parse(response.Content ?? StandardValues.MissingValues.ResponseContent);
                    BearerToken = responseContent.Property("token")?.Value.ToString() ?? StandardValues.MissingValues.BearerToken;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {BearerToken}");

                    JObject infoContent = JObject.Parse(responseContent.Property("info")?.Value.ToString() ?? StandardValues.MissingValues.RelatedContent);
                    expiryTime = DateTime.SpecifyKind(DateTime.Parse(infoContent.Property("expires")?.Value.ToString()), DateTimeKind.Utc);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {expiryTime}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return expiryTime;
        }

        /// <summary>
        /// Returns a list of users from the API.
        /// </summary>
        public async Task<RestResponse?> GetUsers()
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Returns a list of user settings from the API for a given user.
        /// </summary>
        public async Task<RestResponse?> GetUserSettings(int userId)
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Returns a list of servers from the API.
        /// </summary>
        public async Task<RestResponse?> GetServers()
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Returns a list of server status from the API for a given component.
        /// </summary>
        public async Task<RestResponse?> GetServerStatuses(string component)
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Updates the given user setting in the API.
        /// </summary>
        public async Task<RestResponse?> UpdateUserSettings(int userSettingsId, string value)
        {
            RestResponse? response = null;

            try
            {
                string url = BuildURL("/usersettings", userSettingsId, null, true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = JObject.Parse(await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateUserSettings.json"));
                body.Property("value").Value = value;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Updates the given user in the API.
        /// </summary>
        public async Task<RestResponse?> UpdateUser(UserModel user)
        {
            RestResponse? response = null;

            try
            {
                string url = BuildURL("/user", user.UserId, null, true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = JObject.Parse(await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateUser.json"));
                body.Property("Username").Value = user.Username;
                body.Property("Password").Value = user.Password;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Returns a list of alerts from the API.
        /// </summary>
        public async Task<RestResponse?> GetAlerts(int pageNumber)
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Returns a given alert from the API.
        /// </summary>
        public async Task<RestResponse?> GetAlert(int alertId)
        {
            RestResponse? response = null;

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

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Updates the given alert in the API.
        /// </summary>
        public async Task<RestResponse?> UpdateAlert(int alertId, string status)
        {
            RestResponse? response = null;

            try
            {
                string url = BuildURL("/serverstatus/serveralert", alertId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = JObject.Parse(await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\UpdateAlert.json"));
                body.Property("status").Value = status;

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Adds the given alert to the API.
        /// </summary>
        public async Task<RestResponse?> RegisterAlert(APINewAlertsModel alert)
        {
            RestResponse? response = null;

            try
            {
                string url = BuildURL("/serverstatus/serveralert");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = JObject.Parse(await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\RegisterAlert.json"));
                body.Property("reporter").Value = alert.Reporter;
                body.Property("component").Value = alert.Component;
                body.Property("componentStatus").Value = alert.ComponentStatus;
                body.Property("alertStatus").Value = alert.AlertStatus;
                body.Property("hostName").Value = alert.HostName;
                body.Property("game").Value = alert.Game;
                body.Property("gameVersion").Value = alert.GameVersion;

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Adds the given event to the API.
        /// </summary>
        public async Task<RestResponse?> RegisterServerEvent(APIStatusModel status)
        {
            RestResponse? response = null;

            try
            {
                string url = BuildURL("/serverstatus/serverevent");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", SharedSettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject body = JObject.Parse(await _FileSystem.ReadAllText($@"{SharedSettings.PayloadLocation}\RegisterServerEvent.json"));
                body.Property("component").Value = status.Component;
                body.Property("status").Value = status.Status;
                body.Property("hostName").Value = status.Server.HostName;
                body.Property("game").Value = status.Server.Game;
                body.Property("gameVersion").Value = status.Server.GameVersion;

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body.ToString(), ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
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
