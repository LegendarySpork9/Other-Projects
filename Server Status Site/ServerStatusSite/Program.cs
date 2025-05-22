using ServerStatusSite.Components;
using ServerStatusSite.Middleware;
using ServerStatusSite.Models;
using ServerStatusSite.Services;

namespace ServerStatusSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            AppSettingsModel appSettings = new();

            builder.Configuration.Bind("AppSettings", appSettings);
            builder.Services.AddSingleton(appSettings);
            builder.Services.AddSingleton<APIService>();

            builder.Services.AddScoped<UserModel>();

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
