using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class RegisterAlert : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private SharedSettingsModel SharedSettings { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private List<ServerModel> Servers = [];
        private List<string> ServersNames = [];
        private string Server { get; set; } = string.Empty;
        private string Component { get; set; } = string.Empty;
        private string ComponentStatus { get; set; } = string.Empty;
        private bool ShowError { get; set; } = false;
        private bool Loading { get; set; } = false;

        // Loads the servers from the API.
        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Register Alerts Page");

            Servers = APIService.GetServers();

            foreach (ServerModel server in Servers)
            {
                ServersNames.Add($"{server.Game} ({server.GameVersion})");
            }
        }

        // Returns the css to change the page to dark mode.
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

        // Adds an alert to the API.
        private async Task RegisterClick()
        {
            APIAlertsModel alerts = await APIService.GetAlertsAsync(1);
            AlertModel? alert = alerts.Alerts.Find(c => c.Server == Server && c.Component == Component && c.AlertStatus != "Resolved");

            if (alert == null)
            {
                DiscordService _discordService = new(SharedSettings);
                _discordService.SetLogger(Logger);

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting Alert Register");

                string[] gameDetails = Server.Split('(');
                ServerModel? server = Servers.Find(c => c.Game == gameDetails[0].Trim() && c.GameVersion == gameDetails[1].Replace(")", ""));

                APINewAlertsModel newAlert = new()
                {
                    Reporter = User.DiscordName,
                    Component = Component,
                    ComponentStatus = ComponentStatus,
                    AlertStatus = "Reported",
                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                    Game = server.Game ?? StandardValues.MissingValues.Game,
                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
                };

                if (await APIService.RegisterAlertAsync(newAlert))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Alert Registered");
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Alert Register Complete");

                if (SharedSettings.RecipientIds.Contains(','))
                {
                    await _discordService.SendNotificationAsync(SharedSettings.RecipientIds.Split(',')[0], $"{User.DiscordName} has reported an issue with the {Server} server. {Component}: {ComponentStatus}");
                }

                else
                {
                    await _discordService.SendNotificationAsync(SharedSettings.RecipientIds, $"{User.DiscordName} has reported an issue with the {Server} server. {Component}: {ComponentStatus}");
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