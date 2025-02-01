using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        public UserModel User { get; set; }

        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

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

        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
