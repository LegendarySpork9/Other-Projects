using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages.Alerts
{
    public partial class EditAlert : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private AlertModel Alert { get; set; } = new()
        {
            Id = 1,
            Occured = DateTime.UtcNow,
            Reporter = "Hello",
            Component = "PC",
            ComponentStatus = "Offline",
            AlertStatus = "Resolved"
        };
        private int AlertId { get; set; } = 5;

        protected override void OnInitialized()
        {
            Uri uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("alertId", out var alertId))
            {
                AlertId = int.Parse(alertId);
            }
        }

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

        private void SaveClick()
        {
            Navigation.NavigateTo($"/editalert?alertId={AlertId}&saved=true");
        }
    }
}