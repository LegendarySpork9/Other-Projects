using Microsoft.AspNetCore.Components;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages
{
    public partial class Account
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

        private void SaveClick()
        {
            User.Username = Username;
            User.Password = Password;
            User.DiscordName = DiscordName;
            User.DarkMode = DarkMode;
        }
    }
}