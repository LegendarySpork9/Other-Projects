using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class EditAlert : ComponentBase
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
        private AlertModel Alert { get; set; } = new();
        private int AlertId { get; set; } = 0;
        private bool Loading { get; set; } = false;

        // Gets the data about the given alert.
        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Edit Alerts Page");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Url: {Navigation.Uri}");

            Uri uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("alertId", out var alertId))
            {
                AlertId = int.Parse(alertId.ToString() ?? "0");

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {AlertId}");
            }

            Alert = APIService.GetAlert(AlertId);
        }

        // Returns the CSS to change the page to dark mode.
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

        // Updates the alert data then sends the user back to the alerts page. 
        private async Task SaveClick()
        {
            DiscordService _discordService = new(SharedSettings);
            _discordService.SetLogger(Logger);

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting Alert Save");

            if (await APIService.UpdateAlert(AlertId, Alert.AlertStatus))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Alert Status Updated");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Alert Save Complete");

            if (SharedSettings.RecipientIds.Contains(','))
            {
                await _discordService.SendNotificationAsync(SharedSettings.RecipientIds.Split(',')[1], $"{User.DiscordName} has updated the alert for {Alert.Server} - {Alert.Component} to the status {Alert.AlertStatus}.");
            }

            else
            {
                await _discordService.SendNotificationAsync(SharedSettings.RecipientIds, $"{User.DiscordName} has updated the alert for {Alert.Server} - {Alert.Component} to the status {Alert.AlertStatus}.");
            }

            Navigation.NavigateTo("/alerts");
        }
    }
}