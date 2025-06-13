using ServerSiteCommon.Models;

namespace ServerStatusSite.Middleware
{
    public class URLValidationMiddleware
    {
        private readonly SharedSettingsModel SharedSettings;
        private readonly RequestDelegate Next;

        public URLValidationMiddleware(SharedSettingsModel sharedSettings, RequestDelegate next)
        {
            SharedSettings = sharedSettings;
            Next = next;
        }

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
