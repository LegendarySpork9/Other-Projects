using ServerStatusSite.Components;
using ServerStatusSite.Converters;
using ServerStatusSite.Middleware;
using ServerStatusSite.Models;
using ServerStatusSite.Models.Data;
using ServerStatusSite.Services;

namespace ServerStatusSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));

            LoggerService _loggerService = new();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Starting Website");

            var builder = WebApplication.CreateBuilder(args);

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Created Builder");

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            AppSettingsModel appSettings = new();

            builder.Configuration.Bind("AppSettings", appSettings);

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Loaded Configuration");

            builder.Services.AddSingleton(appSettings);
            builder.Services.AddSingleton<APIService>();
            builder.Services.AddSingleton<LoggerService>();
            builder.Services.AddScoped<UserModel>();
            builder.Services.AddHttpContextAccessor();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Configured Services");

            var app = builder.Build();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Built Application");

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Configured HTTPS Redirection");

            app.UseStaticFiles();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Configured Static Files");

            app.UseAntiforgery();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Configured Antiforgery");

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Mapped Razor Components with Interactive Server Render Mode");

            app.UseMiddleware<URLValidationMiddleware>();

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Configured MIddleware");
            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Running Website");

            app.Run();
        }
    }
}
