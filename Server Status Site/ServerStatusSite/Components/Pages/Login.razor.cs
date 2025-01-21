using Microsoft.AspNetCore.Components;

namespace ServerStatusSite.Components.Pages
{
    public partial class Login
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        private string ReturnUrl { get; set; } = "/";
        private string Username { get; set; }
        private string Password { get; set; }
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
            if (Username == "Hello" && Password == "Matey")
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
