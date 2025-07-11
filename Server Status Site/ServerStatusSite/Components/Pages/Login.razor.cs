using Microsoft.AspNetCore.Components;
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Functions;

namespace ServerStatusSite.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private LoggerService Logger { get; set; }
        [Inject]
        private APIService APIService { get; set; }
        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserModel User { get; set; }
        private string ReturnUrl { get; set; } = "/";
        private bool ShowError { get; set; } = false;
        private bool Loading { get; set; } = false;

        // Captures the URL the user was trying to access and sets the API logger.
        protected override void OnInitialized()
        {
            if (HttpContextAccessor != null && HttpContextAccessor.HttpContext != null && HttpContextAccessor.HttpContext.Connection != null && HttpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
            {
                Logger.ChangeIdentifier(HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Login Page");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Url: {Navigation.Uri}");

                Uri uri = Navigation.ToAbsoluteUri(Navigation.Uri);
                var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

                if (queryParams.TryGetValue("returnUrl", out var returnUrl))
                {
                    ReturnUrl = returnUrl;

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Return Url: {ReturnUrl}");
                }

                APIService.SetLogger(Logger);
            }
        }

        // Checks the user details and sends the user to the return URL.
        private async Task LoginClick()
        {
            HashFunction _hasFunction = new();

            Loading = true;
            StateHasChanged();

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Attempting Login");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Username: {User.Username}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Password: {User.Password}");

            await APIService.AuthoriseAsync();
            List<UserModel> users = await APIService.GetUsersAsync();
            UserModel user = users.Find(c => c.Username == User.Username && c.Password == _hasFunction.HashString(User.Password));

            if (user != null)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Login Successful.");
                Logger.ChangeIdentifier(user.Username);
                APIService.SetLogger(Logger);
                User.UpdateModel(APIService.GetUserSettings(user));
                Navigation.NavigateTo(ReturnUrl);
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, "Login Failed.");
                ShowError = true;
            }

            Loading = false;
        }
    }
}
