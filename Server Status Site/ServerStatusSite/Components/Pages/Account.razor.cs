using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages
{
    public partial class Account : ComponentBase
    {
        [Inject] 
        private UserModel User { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string DiscordName { get; set; }
        private bool DarkMode { get; set; }

        protected override void OnInitialized()
        {
            Username = User.Username;
            Password = User.Password;
            DiscordName = User.DiscordName ?? User.Username;
            DarkMode = User.DarkMode;
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
            User.Username = Username;
            User.Password = Password;
            User.DiscordName = DiscordName;
            User.DarkMode = DarkMode;
        }
    }
}