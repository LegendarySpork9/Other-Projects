using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Converters;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class Alerts : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private SharedSettingsModel SharedSettings { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private APIAlertsModel ReportedAlerts = new();
        private Timer RefreshTimer { get; set; } = new();
        private DateTime NextElapse;
        private int PageNumber = 1;

        // Configures the timer and loads the alerts from the API.
        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Alerts Page");

            RefreshTimer = new()
            {
                AutoReset = false
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {SharedSettings.RefreshTime} minutes");

            ReportedAlerts = APIService.GetAlerts(PageNumber);

            DateTime currentTime = DateTime.UtcNow;
            NextElapse = currentTime.AddMinutes(SharedSettings.RefreshTime).AddMilliseconds(-currentTime.Millisecond);

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        // Returns the CSS to change the page to dark mode.
        private string GetStyle(string? component = null)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Data" => _styleConverter.GetTableRowDarkMode(User.DarkMode),
                _ => _styleConverter.GetTableDarkMode(User.DarkMode)
            };
        }

        // Sends the user to the register alert page.
        private void RegisterAlert()
        {
            Navigation.NavigateTo("/registeralert");
        }

        // Loads the previous page of alerts from the API.
        private void PreviousPage()
        {
            PageNumber--;

            ReportedAlerts = APIService.GetAlerts(PageNumber);
        }

        // Loads the next page of alerts from the API.
        private void NextPage()
        {
            PageNumber++;

            ReportedAlerts = APIService.GetAlerts(PageNumber);
        }

        // Sends the user to the edit alert page.
        private void OpenClick(AlertModel alert)
        {
            if (User.Admin)
            {
                Navigation.NavigateTo($"/editalert?alertId={alert.Id}");
            }
        }

        // Loads the alerts from the API.
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            NextElapse = NextElapse.AddMinutes(SharedSettings.RefreshTime);
            ReportedAlerts = APIService.GetAlerts(PageNumber);

            InvokeAsync(StateHasChanged);

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }
    }
}
