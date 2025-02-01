using Microsoft.AspNetCore.Components;
using ServerStatusSite.Models;

namespace ServerStatusSite.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private string ReturnUrl { get; set; } = "/";
        private bool ShowError { get; set; } = false;

        protected override void OnInitialized()
        {
            Uri uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("returnUrl", out var returnUrl))
            {
                ReturnUrl = returnUrl;
            }
        }

        private void LoginClick()
        {
            if (User.Username == "Hello" && User.Password == "Matey")
            {
                Navigation.NavigateTo(ReturnUrl);
            }

            else
            {
                ShowError = true;
            }
        }
    }
}
