namespace ServerStatusSite.Models
{
    public class UserModel
    {
        public event Action? OnDarkModeChanged;
        public bool _DarkMode = false;
        public string Username { get; set; }
        public string Password { get; set; }
        public string DiscordName { get; set; }
        public bool DarkMode
        {
            get => _DarkMode;
            set
            {
                _DarkMode = value;
                OnDarkModeChanged?.Invoke();
            }
        }
        public bool Admin { get; set; }
    }
}
