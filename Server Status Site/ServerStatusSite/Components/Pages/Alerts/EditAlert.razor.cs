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
        private AlertModel Alert { get; set; }
        private int AlertId { get; set; } = 0;

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Edit Alerts Page");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Url: {Navigation.Uri}");

            Uri uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("alertId", out var alertId))
            {
                AlertId = int.Parse(alertId);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Alert Id: {AlertId}");
            }

            Alert = APIService.GetAlert(AlertId);
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

        private void SaveClick()
        {
            DiscordService _discordService = new(Logger, SharedSettings);

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting Alert Save");

            if (APIService.UpdateAlert(AlertId, Alert.AlertStatus))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Alert Status Updated");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Alert Save Complete");

            if (SharedSettings.RecipientIds.Contains(','))
            {
                _discordService.SendNotification(SharedSettings.RecipientIds.Split(',')[1], $"{User.DiscordName} has updated the alert for {Alert.Server} - {Alert.Component} to the status {Alert.AlertStatus}.");
            }

            else
            {
                _discordService.SendNotification(SharedSettings.RecipientIds, $"{User.DiscordName} has updated the alert for {Alert.Server} - {Alert.Component} to the status {Alert.AlertStatus}.");
            }

            Navigation.NavigateTo("/alerts");
        }
    }
}