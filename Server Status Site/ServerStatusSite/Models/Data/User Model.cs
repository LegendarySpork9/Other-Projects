namespace ServerStatusSite.Models.Data
{
    public class UserModel
    {
        public event Action? OnDarkModeChanged;
        private bool _DarkMode = false;

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DiscordName { get; set; }
        public bool Admin { get; set; }
        public bool DarkMode
        {
            get => _DarkMode;
            set
            {
                _DarkMode = value;
                OnDarkModeChanged?.Invoke();
            }
        }

        public void UpdateModel(UserModel user)
        {
            UserId = user.UserId;
            DiscordName = user.DiscordName;
            Admin = user.Admin;
            DarkMode = user.DarkMode;
        }
    }
}
