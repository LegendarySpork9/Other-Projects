// Copyright © - 05/10/2025 - Toby Hunter
using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Models.Data;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        public UserModel User { get; set; }

        // Subscribes the layout to the DarkMode event.
        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        // Returns the CSS to change the layout to dark mode.
        private string GetStyle(string component)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Body" => _styleConverter.GetBodyDarkMode(User.DarkMode),
                "Bar" => _styleConverter.GetTopBarDarkMode(User.DarkMode),
                "Link" => _styleConverter.GetTopNavLinkDarkMode(User.DarkMode),
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
