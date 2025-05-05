using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class RegisterAlert : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private string Component { get; set; }
        private string ComponentStatus { get; set; }

        public string GetStyle(string component)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Form" => _styleConverter.GetFormDarkMode(User.DarkMode),
                "Input" => _styleConverter.GetInputDarkMode(User.DarkMode),
                _ => string.Empty
            };
        }

        private void RegisterClick()
        {
            AlertModel alert = new()
            {
                Id = 1,
                Occured = DateTime.UtcNow,
                Reporter = User.DiscordName,
                Component = Component,
                ComponentStatus = ComponentStatus,
                AlertStatus = "Reported"
            };

            Navigation.NavigateTo($"/registeralert?alertId={alert.Id}");
        }
    }
}