// Copyright © - Unpublished - Toby Hunter
namespace ServerStatusCommon.Models.Responses
{
    /// <summary>
    /// Stores the user API response.
    /// </summary>
    public class UserModel
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required List<string> Scopes { get; set; }
        public Dictionary<string, string> Settings { get; set; } = [];

        public event Action? OnDarkModeChanged;

        public bool DarkMode
        {
            get => Settings.TryGetValue("DarkMode", out var val) && bool.Parse(val);
            set
            {
                Settings["DarkMode"] = value.ToString();
                OnDarkModeChanged?.Invoke();
            }
        }

        /// <summary>
        /// Updates the user model.
        /// </summary>
        public void UpdateModel(UserModel user)
        {
            Id = user.Id;
            Settings["DiscordName"] = user.Settings["DiscordName"];
            Settings["Admin"] = user.Settings["Admin"];
            Settings["DarkMode"] = user.Settings["DarkMode"];
        }
    }
}
