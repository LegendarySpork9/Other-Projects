using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;
using ServerStatusSite.Models.API;
using ServerStatusSite.Models.Data;
using ServerStatusSite.Services;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class RegisterAlert : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private AppSettingsModel AppSettings { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private List<ServerModel> Servers = [];
        private List<string> ServersNames = [];
        private string Server { get; set; }
        private string Component { get; set; }
        private string ComponentStatus { get; set; }
        private bool ShowError { get; set; } = false;

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Register Alerts Page");

            Servers = APIService.GetServers();

            foreach (ServerModel server in Servers)
            {
                ServersNames.Add($"{server.Game} ({server.GameVersion})");
            }
        }

        public string GetStyle(string component)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Form" => _styleConverter.GetFormDarkMode(User.DarkMode),
                "Input" => _styleConverter.GetInputDarkMode(User.DarkMode),
                _ => string.Empty
            };
        }

        private void RegisterClick()
        {
            APIAlertsModel alerts = APIService.GetAlerts(1);
            AlertModel alert = alerts.Alerts.Find(c => c.Server == Server && c.Component == Component && c.AlertStatus != "Resolved");

            if (alert == null)
            {
                DiscordService _discordService = new(Logger, AppSettings);

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting Alert Register");

                string[] gameDetails = Server.Split('(');
                ServerModel server = Servers.Find(c => c.Game == gameDetails[0].Trim() && c.GameVersion == gameDetails[1].Replace(")", ""));

                APINewAlertsModel newAlert = new()
                {
                    Reporter = User.DiscordName,
                    Component = Component,
                    ComponentStatus = ComponentStatus,
                    AlertStatus = "Reported",
                    HostName = server.HostName,
                    Game = server.Game,
                    GameVersion = server.GameVersion
                };

                if (APIService.RegisterAlert(newAlert))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Alert Registered");
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Alert Register Complete");

                if (AppSettings.RecipientIds.Contains(','))
                {
                    _discordService.SendNotification(AppSettings.RecipientIds.Split(',')[0], $"{User.DiscordName} has reported an issue with the {Server} server. {Component}: {ComponentStatus}");
                }

                else
                {
                    _discordService.SendNotification(AppSettings.RecipientIds, $"{User.DiscordName} has reported an issue with the {Server} server. {Component}: {ComponentStatus}");
                }

                Navigation.NavigateTo("/alerts");
            }

            else
            {
                ShowError = true;
            }
        }
    }
}