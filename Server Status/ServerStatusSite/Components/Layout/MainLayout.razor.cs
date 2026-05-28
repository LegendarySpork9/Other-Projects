// Copyright © - 05/10/2025 - Toby Hunter
using Microsoft.AspNetCore.Components;
using ServerStatusCommon.Models.Responses;
using ServerStatusSite.Converters;

namespace ServerStatusSite.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        public UserModel User { get; set; } = default!;

        /// <summary>
        /// Subscribes the layout to the DarkMode event.
        /// </summary>
        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        /// <summary>
        /// Returns the CSS to change the layout to dark mode.
        /// </summary>
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

        /// <summary>
        /// Unsubscribes the layout from the DarkMode event.
        /// </summary>
        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
