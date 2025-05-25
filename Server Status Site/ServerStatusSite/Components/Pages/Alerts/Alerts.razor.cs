using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models.API;
using ServerStatusSite.Models.Data;
using ServerStatusSite.Services;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class Alerts : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private APIAlertsModel ReportedAlerts = new();
        private int PageNumber = 1;

        protected override void OnInitialized()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Alerts Page");

            ReportedAlerts = APIService.GetAlerts(PageNumber);
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
    }
}
