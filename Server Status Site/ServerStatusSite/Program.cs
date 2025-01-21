using ServerStatusSite.Components;
using ServerStatusSite.Middleware;
using ServerStatusSite.Models;

namespace ServerStatusSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            AppSettingsModel appSettings = new();

            builder.Configuration.Bind("AppSettings", appSettings);
            builder.Services.AddSingleton(appSettings);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.UseMiddleware<URLValidationMiddleware>();

            app.Run();
        }
    }
}
