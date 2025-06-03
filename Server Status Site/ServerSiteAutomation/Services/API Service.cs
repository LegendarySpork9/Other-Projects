using Newtonsoft.Json.Linq;
using RestSharp;
using ServerSiteAutomation.Converters;
using ServerSiteAutomation.Models;
using ServerSiteAutomation.Models.API;
using ServerSiteAutomation.Models.Data;

namespace ServerSiteAutomation.Services
{
    public class APIService
    {
        private readonly LoggerService Logger = new();
        private readonly string[] Endpoints = AppSettingsModel.Endpoints.Split(',');
        private string BearerToken { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int RetryCount { get; set; } = 0;

        public void Authorise()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Authorisation:")).Replace("Authorisation:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", AppSettingsModel.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", File.ReadAllText($@"{AppSettingsModel.PayloadLocation}\Authorise.json"), ParameterType.RequestBody);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {File.ReadAllText($@"{AppSettingsModel.PayloadLocation}\Authorise.json")}");
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
                    Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to obtain Bearer token from API");
            }
        }

        public List<ServerModel> GetServers()
        {
            APIConverter _apiConverter = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching servers from API");

            if (ExpiryTime <= DateTime.UtcNow)
            {
                Authorise();
            }

            List<ServerModel> servers = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Servers:")).Replace("Servers:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint + "?IsActive=true";

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

        public List<APIStatusModel> GetServerStatuses(string component)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching server statuses from API");

            if (ExpiryTime <= DateTime.UtcNow)
            {
                Authorise();
            }

            List<APIStatusModel> statuses = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Statuses:")).Replace("Statuses:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint + $"?Component={component}";

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

        public APIAlertsModel GetAlerts(int pageNumber)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching alerts from API");

            if (ExpiryTime <= DateTime.UtcNow)
            {
                Authorise();
            }

            APIAlertsModel alerts = new();

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint + $"?PageNumber={pageNumber}";

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

        public bool RegisterAlert(APINewAlertsModel alert)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registering alert in API");

            if (ExpiryTime <= DateTime.UtcNow)
            {
                Authorise();
            }

            bool registered = false;

            try
            {
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Alerts:")).Replace("Alerts:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettingsModel.PayloadLocation}\RegisterAlert.json"));
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
                        Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to register alert in API");
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
                string authEndpoint = Array.Find(Endpoints, e => e.StartsWith("Statuses:")).Replace("Statuses:", "");
                string url = AppSettingsModel.BaseURL + authEndpoint;

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
                client.AddDefaultHeader("Accept", "application/json");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                JObject json = JObject.Parse(File.ReadAllText($@"{AppSettingsModel.PayloadLocation}\RegisterServerEvent.json"));
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
