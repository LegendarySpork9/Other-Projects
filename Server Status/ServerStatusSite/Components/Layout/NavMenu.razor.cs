// Copyright © - 05/10/2025 - Toby Hunter
using Microsoft.AspNetCore.Components;
using ServerStatusCommon.Models.Data;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        private UserModel User { get; set; } = default!;

        // Subscribes the layout to the DarkMode event.
        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        // Returns the CSS to change the menu to dark mode.
        private string GetStyle(string? component = null)
        {
            return component switch
            {
                "Corner" => StyleConverter.GetTopBarDarkMode(User.DarkMode),
                _ => StyleConverter.GetNavMenuDarkMode(User.DarkMode)
            };
        }

        // Unsubscribes the layout from the DarkMode event.
        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
