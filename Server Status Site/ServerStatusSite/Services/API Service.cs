using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

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
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API.");

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

                if (response.IsSuccessStatusCode)
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

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API.");
        }

        public List<UserModel> GetUsers()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API.");

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

                if (response.IsSuccessStatusCode)
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

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched users from API.");
            return users;
        }
    }
}
