using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        private UserModel User { get; set; }
        private List<ServerModel> Servers = [];

        protected override void OnInitialized()
        {
            for (int i = 0; i < 2; i++)
            {
                List<StatusModel> statuses = [];

                StatusModel status = new()
                {
                    Status = "Online",
                    StatusClass = "online"
                };

                statuses.Add(status);

                status = new()
                {
                    Status = "Unknown",
                    StatusClass = "unknown"
                };

                statuses.Add(status);

                status = new()
                {
                    Status = "Offline",
                    StatusClass = "offline"
                };

                statuses.Add(status);

                ServerModel server = new()
                {
                    HostName = "HunterNas",
                    Game = "Minecraft",
                    GameVersion = "1.12.2",
                    IPAddress = "25.35.45.248",
                    Statuses = statuses
                };

                Servers.Add(server);
            }
        }

        private string GetStyle()
        {
            StyleConverter _styleConverter = new();

            return _styleConverter.GetTableDarkMode(User.DarkMode);
        }
    }
}
