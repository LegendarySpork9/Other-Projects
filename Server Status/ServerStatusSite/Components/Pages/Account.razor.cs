// Copyright © - 05/10/2025 - Toby Hunter
using Microsoft.AspNetCore.Components;
using ServerStatusCommon.Converters;
using ServerStatusCommon.Models.Data;
using ServerStatusCommon.Abstractions;
using ServerStatusCommon.Services;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Pages
{
    public partial class Account : ComponentBase
    {
        [Inject]
        private ILoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private UserModel User { get; set; } = default!;

        private string Username { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        private string DiscordName { get; set; } = string.Empty;
        private bool DarkMode { get; set; }
        private bool Loading { get; set; } = false;

        /// <summary>
        /// Loads the user data of the logged in user.
        /// </summary>
        protected override void OnInitialized()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Home Page");

            Username = User.Username;
            Password = User.Password;
            DiscordName = User.DiscordName ?? User.Username;
            DarkMode = User.DarkMode;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Username: {Username}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Password: {Password}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Discord: {DiscordName}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Dark Mode: {DarkMode}");
        }

        /// <summary>
        /// Returns the CSS to change the page to dark mode.
        /// </summary>
        public string GetStyle(string component)
        {
            return component switch
            {
                "Form" => StyleConverter.GetFormDarkMode(User.DarkMode),
                "Input" => StyleConverter.GetInputDarkMode(User.DarkMode),
                _ => string.Empty
            };
        }

        /// <summary>
        /// Updates the user data.
        /// </summary>
        private async Task SaveClick()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting User Save");

            Loading = true;
            StateHasChanged();

            if (!string.IsNullOrWhiteSpace(Username) && Username != User.Username)
            {
                User.Username = Username;
                
                if (await APIService.UpdateUser(User))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Username Updated");
                }
            }

            if (!string.IsNullOrWhiteSpace(Password) && Password != User.Password)
            {
                User.Password = Password;

                if (await APIService.UpdateUser(User))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Password Updated");
                }
            }

            if (!string.IsNullOrWhiteSpace(DiscordName) && DiscordName != User.DiscordName)
            {
                User.DiscordName = DiscordName;

                int userSettingsId = await APIService.GetUserSettingId(User.UserId, "DiscordName");

                if (await APIService.UpdateUserSettings(userSettingsId, DiscordName))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Discord Updated");
                }
            }

            if (DarkMode != User.DarkMode)
            {
                User.DarkMode = DarkMode;

                int userSettingsId = await APIService.GetUserSettingId(User.UserId, "DarkMode");

                if (await APIService.UpdateUserSettings(userSettingsId, DarkMode.ToString()))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Discord Updated");
                }
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, "User Save Complete");

            Loading = false;
        }
    }
}