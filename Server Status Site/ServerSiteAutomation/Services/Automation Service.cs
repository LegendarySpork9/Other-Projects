using ServerSiteAutomation.Converters;
using ServerSiteAutomation.Functions;
using ServerSiteAutomation.Models;
using ServerSiteAutomation.Models.API;
using ServerSiteAutomation.Models.Data;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerSiteAutomation.Services
{
    public class AutomationService
    {
        private readonly LoggerService Logger = new();
        private readonly APIService APIService = new();
        private Timer RefreshTimer;
        private DateTime NextElapse;

        public void Setup()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configuring Automation Service");

            RefreshTimer = new()
            {
                AutoReset = false
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {AppSettingsModel.RefreshTime} minutes");
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configured Automation Service");
        }

        public void Start()
        {
            AutomationFunction _automationFunction = new();

            Run();

            DateTime currentTime = DateTime.UtcNow;
            NextElapse = currentTime.AddMinutes(AppSettingsModel.RefreshTime).AddMilliseconds(-currentTime.Millisecond);

            RefreshTimer.Interval = _automationFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            AutomationFunction _automationFunction = new();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, "Timer Triggered");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Token Expiry: {APIService.ExpiryTime}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Time: {DateTime.UtcNow}");

            NextElapse = NextElapse.AddMinutes(AppSettingsModel.RefreshTime);

            Run();

            RefreshTimer.Interval = _automationFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        private void Run()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Running Automatic Status Checks");

            List<ServerModel> servers = APIService.GetServers();
            List<APIStatusModel> pcStatuses = APIService.GetServerStatuses("PC Status");
            List<APIStatusModel> hamachiStatuses = APIService.GetServerStatuses("Hamachi Status");
            List<APIStatusModel> serverStatuses = APIService.GetServerStatuses("Server Status");
            APIAlertsModel alerts = APIService.GetAlerts(1);

            foreach (ServerModel server in servers)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Checking Status for {server.HostName} - {server.Game} ({server.GameVersion})");

                APIStatusModel pcStatus = pcStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);
                APIStatusModel hamachiStatus = hamachiStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);
                APIStatusModel serverStatus = serverStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current PC Status: {pcStatus.Status}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Hamachi Status: {hamachiStatus.Status}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Server Status: {serverStatus.Status}");

                DateTime refreshPeriod = DateTime.UtcNow.AddMinutes(-AppSettingsModel.RefreshTime);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Period: {refreshPeriod}");

                if (pcStatus.DateOccured < refreshPeriod)
                {
                    if (server.Statuses[0].Status == "Online")
                    {
                        server.Statuses[0].Status = "Unknown";

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Updated PC Status to Unknown");
                    }

                    AlertsHandler(alerts.Alerts, server, pcStatus.Component);

                    APIStatusModel newStatus = new()
                    {
                        Component = pcStatus.Component,
                        Status = server.Statuses[0].Status,
                        Server = new APIRelatedServerModel
                        {
                            HostName = server.HostName,
                            Game = server.Game,
                            GameVersion = server.GameVersion
                        }
                    };
                    
                    if (APIService.RegisterServerEvent(newStatus))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Server Event Registered");
                    }
                }

                if (hamachiStatus.DateOccured < refreshPeriod)
                {
                    if (server.Statuses[1].Status == "Online")
                    {
                        server.Statuses[1].Status = "Unknown";

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Updated Hamachi Status to Unknown");
                    }

                    AlertsHandler(alerts.Alerts, server, hamachiStatus.Component);

                    APIStatusModel newStatus = new()
                    {
                        Component = hamachiStatus.Component,
                        Status = server.Statuses[1].Status,
                        Server = new APIRelatedServerModel
                        {
                            HostName = server.HostName,
                            Game = server.Game,
                            GameVersion = server.GameVersion
                        }
                    };

                    if (APIService.RegisterServerEvent(newStatus))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Server Event Registered");
                    }
                }

                if (serverStatus.DateOccured < refreshPeriod)
                {
                    if (server.Statuses[2].Status == "Online")
                    {
                        server.Statuses[2].Status = "Unknown";

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Updated Server Status to Unknown");
                    }

                    AlertsHandler(alerts.Alerts, server, serverStatus.Component);

                    APIStatusModel newStatus = new()
                    {
                        Component = serverStatus.Component,
                        Status = server.Statuses[2].Status,
                        Server = new APIRelatedServerModel
                        {
                            HostName = server.HostName,
                            Game = server.Game,
                            GameVersion = server.GameVersion
                        }
                    };

                    if (APIService.RegisterServerEvent(newStatus))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Server Event Registered");
                    }
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Checked Status for {server.HostName} - {server.Game} ({server.GameVersion})");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Ran Automatic Status Checks");
        }

        private void AlertsHandler(List<AlertModel> alertData, ServerModel server, string component)
        {
            DiscordService _discordService = new();

            foreach (AlertModel alert in alertData)
            {
                if (alert.Server == $"{server.Game} ({server.GameVersion})" && alert.Component == component)
                {
                    if (alert.AlertStatus == "Resolved")
                    {
                        APINewAlertsModel newAlert = new()
                        {
                            Reporter = "Automation",
                            Component = component,
                            ComponentStatus = "Unknown",
                            AlertStatus = "Reported",
                            HostName = server.HostName,
                            Game = server.Game,
                            GameVersion = server.GameVersion
                        };

                        if (APIService.RegisterAlert(newAlert))
                        {
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, "Alert Registered");
                        }

                        _discordService.SendNotification($"Automation has reported an issue with the {server.Game} ({server.GameVersion}) server. {component}: Unknown");
                    }

                    else
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Existing Alert Found");
                    }

                    break;
                }
            }
        }
    }
}
