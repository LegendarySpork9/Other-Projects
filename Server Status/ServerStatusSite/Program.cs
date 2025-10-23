// Copyright © - 05/10/2025 - Toby Hunter
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerStatusSite.Components;
using ServerStatusSite.Middleware;

namespace ServerStatusSite
{
    public class Program
    {
        // Configures the application at startup.
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));

            LoggerService _loggerService = new();

            _loggerService.LogMessage(StandardValues.LoggerValues.Info, "Starting Website");

            var builder = WebApplication.CreateBuilder(args);

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Created Builder");

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            SharedSettingsModel sharedSettings = new();

            builder.Configuration.Bind("AppSettings", sharedSettings);

            _loggerService.LogMessage(StandardValues.LoggerValues.Debug, "Loaded Configuration");

            builder.Services.AddSingleton(sharedSettings);
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
