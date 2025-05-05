using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class Alerts : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private List<AlertModel> ReportedAlerts = [];

        protected override void OnInitialized()
        {
            ReportedAlerts.Add(new AlertModel
            {
                Id = ReportedAlerts.Count + 1,
                Occured = DateTime.UtcNow,
                Reporter = "LegendarySpork9",
                Component = "Hamachi",
                ComponentStatus = "Offline",
                AlertStatus = "Reported"
            });

            ReportedAlerts = ReportedAlerts.OrderByDescending(c => c.Id).ToList();
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

        private void OpenClick(AlertModel alert)
        {
            if (User.Admin)
            {
                Navigation.NavigateTo($"/editalert?alertId={alert.Id}");
            }
        }
    }
}
