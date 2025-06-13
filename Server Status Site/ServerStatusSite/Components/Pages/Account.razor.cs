using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Pages
{
    public partial class Account : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject] 
        private UserModel User { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string DiscordName { get; set; }
        private bool DarkMode { get; set; }

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Home Page");

            Username = User.Username;
            Password = User.Password;
            DiscordName = User.DiscordName ?? User.Username;
            DarkMode = User.DarkMode;

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Username: {Username}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Password: {Password}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Discord: {DiscordName}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Dark Mode: {DarkMode}");
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
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting User Save");
            
            if (!string.IsNullOrWhiteSpace(Username) && Username != User.Username)
            {
                User.Username = Username;
                
                if (APIService.UpdateUser(User))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Username Updated");
                }
            }

            if (!string.IsNullOrWhiteSpace(Password) && Password != User.Password)
            {
                User.Password = Password;

                if (APIService.UpdateUser(User))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Password Updated");
                }
            }

            if (!string.IsNullOrWhiteSpace(DiscordName) && DiscordName != User.DiscordName)
            {
                User.DiscordName = DiscordName;

                int userSettingsId = APIService.GetUserSettingId(User.UserId, "DiscordName");

                if (APIService.UpdateUserSettings(userSettingsId, DiscordName))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Discord Updated");
                }
            }

            if (DarkMode != User.DarkMode)
            {
                User.DarkMode = DarkMode;

                int userSettingsId = APIService.GetUserSettingId(User.UserId, "DarkMode");

                if (APIService.UpdateUserSettings(userSettingsId, DarkMode.ToString()))
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Discord Updated");
                }
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "User Save Complete");
        }
    }
}