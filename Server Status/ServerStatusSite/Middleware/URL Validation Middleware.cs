// Copyright © - 05/10/2025 - Toby Hunter
using ServerStatusCommon.Models;

namespace ServerStatusSite.Middleware
{
    public class URLValidationMiddleware
    {
        private readonly SharedSettingsModel SharedSettings;
        private readonly RequestDelegate Next;

        // Configures the URL middleware.
        public URLValidationMiddleware(SharedSettingsModel sharedSettings, RequestDelegate next)
        {
            SharedSettings = sharedSettings;
            Next = next;
        }

        /// <summary>
        /// Directs the user to the login page if they come from a link outside the website.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            string referer = context.Request.Headers.Referer.ToString();

            if (string.IsNullOrWhiteSpace(referer) || (!referer.Contains(SharedSettings.Domain, StringComparison.OrdinalIgnoreCase) && !referer.Contains("localhost", StringComparison.OrdinalIgnoreCase)))
            {
                if (!context.Request.Path.StartsWithSegments("/login", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect($"/login?returnUrl={context.Request.Path}");
                    return;
                }
            }

            await Next(context);
        }
    }
}
