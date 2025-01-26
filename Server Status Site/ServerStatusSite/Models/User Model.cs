namespace ServerStatusSite.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DiscordName { get; set; }
        public bool DarkMode { get; set; } = false;
    }
}
