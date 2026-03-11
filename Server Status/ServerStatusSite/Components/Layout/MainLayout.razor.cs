// Copyright © - 05/10/2025 - Toby Hunter
using Microsoft.AspNetCore.Components;
using ServerStatusCommon.Models.Data;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        public UserModel User { get; set; } = default!;

        // Subscribes the layout to the DarkMode event.
        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        // Returns the CSS to change the layout to dark mode.
        private string GetStyle(string component)
        {
            return component switch
            {
                "Body" => StyleConverter.GetBodyDarkMode(User.DarkMode),
                "Bar" => StyleConverter.GetTopBarDarkMode(User.DarkMode),
                "Link" => StyleConverter.GetTopNavLinkDarkMode(User.DarkMode),
                _ => String.Empty
            };
        }

        // Unsubscribes the layout from the DarkMode event.
        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
