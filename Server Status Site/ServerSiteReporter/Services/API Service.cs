using Newtonsoft.Json.Linq;
using RestSharp;
using ServerSiteReporter.Converters;
using ServerSiteReporter.Models;
using ServerSiteReporter.Models.API;
using ServerSiteReporter.Models.Data;

namespace ServerSiteReporter.Services
{
    public class APIService
    {
        private readonly LoggerService Logger = new();
        private readonly string[] Endpoints = AppSettingsModel.Endpoints.Split(',');
        private string BearerToken { get; set; }
        public DateTime ExpiryTime { get; set; }

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
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
        }

        public List<ServerModel> GetServers()
        {
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

                    foreach (JObject server in responseContent)
                    {
                        string hostName = server.Property("hostName").Value.ToString();
                        string game = server.Property("game").Value.ToString();
                        string gameVersion = server.Property("gameVersion").Value.ToString();
                        string ipAddress = server.Property("ipAddress").Value.ToString();

                        servers.Add(new ServerModel()
                        {
                            HostName = hostName,
                            Game = game,
                            GameVersion = gameVersion,
                            IPAddress = server.Property("ipAddress").Value.ToString(),
                        });

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Host Name: {hostName}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game: {game}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Game Version: {gameVersion}");
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {server.Property("ipAddress").Value}");
                    }
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Authorise();
                    return GetServers();
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
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Authorise();
                    return RegisterServerEvent(status);
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Registered server event in API");
            return registered;
        }
    }
}
