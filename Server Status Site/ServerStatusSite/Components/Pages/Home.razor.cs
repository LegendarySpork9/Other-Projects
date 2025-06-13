using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Converters;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerStatusSite.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private SharedSettingsModel SharedSettings { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private List<ServerModel> Servers = [];
        private Timer RefreshTimer { get; set; }

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Home Page");

            RefreshTimer = new()
            {
                Interval = SharedSettings.RefreshTime * 1000
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {RefreshTimer.Interval / 1000 / 60} minutes");

            Servers = APIService.GetServers();

            RefreshTimer.Start();
        }

        private string GetStyle()
        {
            StyleConverter _styleConverter = new();

            return _styleConverter.GetTableDarkMode(User.DarkMode);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Servers = APIService.GetServers();
        }
    }
}
