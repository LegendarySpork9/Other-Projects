using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
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
        private Timer RefreshTimer { get; set; } = new();
        private DateTime NextElapse;

        // Configures the timer and loads the servers from the API.
        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Home Page");

            RefreshTimer = new()
            {
                AutoReset = false
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {SharedSettings.RefreshTime} minutes");

            Servers = APIService.GetServers();

            DateTime currentTime = DateTime.UtcNow;
            NextElapse = currentTime.AddMinutes(SharedSettings.RefreshTime).AddMilliseconds(-currentTime.Millisecond);

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        // Returns the CSS to change the page to dark mode.
        private string GetStyle()
        {
            StyleConverter _styleConverter = new();

            return _styleConverter.GetTableDarkMode(User.DarkMode);
        }

        // Loads the servers from the API.
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            NextElapse = NextElapse.AddMinutes(SharedSettings.RefreshTime);
            Servers = APIService.GetServers();

            InvokeAsync(StateHasChanged);

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }
    }
}
