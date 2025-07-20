using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerSiteAutomation.Services
{
    public class AutomationService
    {
        private LoggerService Logger = new();
        private readonly APIService APIService;
        private SharedSettingsModel SharedSettings;
        private Timer RefreshTimer;
        private DateTime NextElapse;

        // Sets the class's global variables.
        public AutomationService(SharedSettingsModel sharedSettings)
        {
            SharedSettings = sharedSettings;
            APIService = new(sharedSettings);
        }

        // Sets the logger.
        public void SetLogger(LoggerService _loggerService)
        {
            Logger = _loggerService;
        }

        // Configures the timer and API service logger.
        public void Setup()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configuring Automation Service");

            RefreshTimer = new()
            {
                AutoReset = false
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {SharedSettings.RefreshTime} minutes");

            APIService.SetLogger(Logger);

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configured Automation Service");
        }

        // Performs the first run and starts the timer.
        public void Start()
        {
            Run();

            DateTime currentTime = DateTime.UtcNow;
            NextElapse = currentTime.AddMinutes(SharedSettings.RefreshTime).AddMilliseconds(-currentTime.Millisecond);

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        // Performs a run then restarts the timer.
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, "Timer Triggered");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Token Expiry: {APIService.ExpiryTime}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Time: {DateTime.UtcNow}");

            NextElapse = NextElapse.AddMinutes(SharedSettings.RefreshTime);

            Run();

            RefreshTimer.Interval = TimerFunction.GetTimerInterval(NextElapse).TotalMilliseconds;
            RefreshTimer.Start();
        }

        // Runs the status checks.
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

                APIStatusModel? pcStatus = pcStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);
                APIStatusModel? hamachiStatus = hamachiStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);
                APIStatusModel? serverStatus = serverStatuses.Find(c => c.Server.HostName == server.HostName && c.Server.Game == server.Game && c.Server.GameVersion == server.GameVersion);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current PC Status: {pcStatus?.Status ?? StandardValues.MissingValues.Status}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Hamachi Status: {hamachiStatus?.Status ?? StandardValues.MissingValues.Status}");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Server Status: {serverStatus?.Status ?? StandardValues.MissingValues.Status}");

                DateTime refreshPeriod = DateTime.UtcNow.AddMinutes(-SharedSettings.RefreshTime);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Refresh Period: {refreshPeriod}");

                if (pcStatus != null && pcStatus.DateOccured < refreshPeriod)
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

                if (hamachiStatus != null && hamachiStatus.DateOccured < refreshPeriod)
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

                if (serverStatus != null && serverStatus.DateOccured < refreshPeriod)
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

        // Raises an alert if an unresolved one is not found.
        private void AlertsHandler(List<AlertModel> alerts, ServerModel server, string component)
        {
            DiscordService _discordService = new(SharedSettings);
            _discordService.SetLogger(Logger);

            foreach (AlertModel alert in alerts)
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

                        _discordService.SendNotification(SharedSettings.RecipientId, $"Automation has reported an issue with the {server.Game} ({server.GameVersion}) server. {component}: Unknown");
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
