using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;
using ServerStatusSite.Models.API;
using ServerStatusSite.Models.Data;
using ServerStatusSite.Services;
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
        private AppSettingsModel AppSettings { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private APIAlertsModel ReportedAlerts = new();
        private Timer RefreshTimer { get; set; }
        private int PageNumber = 1;

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Alerts Page");

            RefreshTimer = new()
            {
                Interval = AppSettings.RefreshTime * 1000
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {RefreshTimer.Interval / 1000 / 60} minutes");

            ReportedAlerts = APIService.GetAlerts(PageNumber);

            RefreshTimer.Start();
        }

        private string GetStyle(string component = null)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Data" => _styleConverter.GetTableRowDarkMode(User.DarkMode),
                _ => _styleConverter.GetTableDarkMode(User.DarkMode)
            };
        }

        private void RegisterAlert()
        {
            Navigation.NavigateTo("/registeralert");
        }

        private void PreviousPage()
        {
            PageNumber--;

            ReportedAlerts = APIService.GetAlerts(PageNumber);
        }

        private void NextPage()
        {
            PageNumber++;

            ReportedAlerts = APIService.GetAlerts(PageNumber);
        }

        private void OpenClick(AlertModel alert)
        {
            if (User.Admin)
            {
                Navigation.NavigateTo($"/editalert?alertId={alert.Id}");
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ReportedAlerts = APIService.GetAlerts(PageNumber);
        }
    }
}
