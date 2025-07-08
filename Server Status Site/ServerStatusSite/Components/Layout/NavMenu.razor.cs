using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Models.Data;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        private UserModel User { get; set; }

        // Subscribes the layout to the DarkMode event.
        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        // Returns the CSS to change the menu to dark mode.
        private string GetStyle(string component = null)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Corner" => _styleConverter.GetTopBarDarkMode(User.DarkMode),
                _ => _styleConverter.GetNavMenuDarkMode(User.DarkMode)
            };
        }

        // Unsubscribes the layout from the DarkMode event.
        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
