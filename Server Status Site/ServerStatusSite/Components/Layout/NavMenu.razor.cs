using Microsoft.AspNetCore.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        private UserModel User { get; set; }

        protected override void OnInitialized()
        {
            User.OnDarkModeChanged += StateHasChanged;
        }

        private string GetStyle(string component = null)
        {
            StyleConverter _styleConverter = new();

            return component switch
            {
                "Corner" => _styleConverter.GetTopBarDarkMode(User.DarkMode),
                _ => _styleConverter.GetNavMenuDarkMode(User.DarkMode)
            };
        }

        public void Dispose()
        {
            User.OnDarkModeChanged -= StateHasChanged;
        }
    }
}
