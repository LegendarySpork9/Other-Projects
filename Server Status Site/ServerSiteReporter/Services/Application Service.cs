using ServerSiteReporter.Converters;
using ServerSiteReporter.Models;
using ServerSiteReporter.Models.API;
using ServerSiteReporter.Models.Data;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerSiteReporter.Services
{
    internal class ApplicationService
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private readonly LoggerService Logger = new();
        private readonly APIService APIService = new();
        private Timer RefreshTimer;

        public void Setup()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configuring Automation Service");

            RefreshTimer = new()
            {
                Interval = AppSettingsModel.RefreshTime * 1000
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {RefreshTimer.Interval / 1000 / 60} minutes");
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configured Automation Service");
        }

        public void Start()
        {
            Run();
            Thread.Sleep(1000);
            RefreshTimer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, "Timer Triggered");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Token Expiry: {APIService.ExpiryTime}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Current Time: {DateTime.UtcNow}");
            Run();
        }

        private void Run()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Running Event Register");

            List<ServerModel> servers = APIService.GetServers();

            foreach (string game in AppSettingsModel.Games)
            {
                string[] gameParts = game.Split('_');
                gameParts[1] = gameParts[1].Replace("(", "").Replace(")", "");

                ServerModel server = servers.Find(c => c.HostName == AppSettingsModel.HostName && c.Game == gameParts[0] && c.GameVersion == gameParts[1]);

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Registering Events for {server.HostName} - {server.Game} ({server.GameVersion})");

                foreach (string component in AppSettingsModel.Components)
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component: {component}");

                    if (component == "PC")
                    {
                        APIStatusModel newStatus = new()
                        {
                            Component = "PC Status",
                            Status = "Online",
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

                    if (component == "Hamachi")
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {server.IPAddress}");

                        string pingStatus = PingAddress(server.IPAddress);

                        if (pingStatus == "Success")
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Hamachi Status",
                                Status = "Online",
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

                        else if (pingStatus == "Failed")
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Hamachi Status",
                                Status = "Offline",
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

                        else
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Hamachi Status",
                                Status = "Unknown",
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
                    }

                    if (component == "Server")
                    {
                        if (ServerRunning($"{server.Game} ({server.GameVersion})"))
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Server Status",
                                Status = "Online",
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

                        else
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Server Status",
                                Status = "Offline",
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
                    }
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Registered Events for {server.HostName} - {server.Game} ({server.GameVersion})");
            }

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Ran Event Register");
        }

        private string PingAddress(string ipAddress)
        {
            string response = string.Empty;

            try
            {
                Ping pingSender = new();

                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Ping Sender");
                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Pinging Address");

                PingReply reply = pingSender.Send(ipAddress);

                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Reply Status: {reply.Status}");

                if (reply.Status == IPStatus.Success)
                {
                    response = "Success";
                }

                else
                {
                    response = "Failed";
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        private bool ServerRunning(string game)
        {
            bool running = false;

            try
            {
                EnumWindows((hWnd, lParam) =>
                {
                    if (!IsWindowVisible(hWnd))
                    {
                        return true;
                    }

                    int length = GetWindowTextLength(hWnd);

                    if (length == 0)
                    {
                        return true;
                    }

                    var builder = new StringBuilder(length + 1);

                    GetWindowText(hWnd, builder, builder.Capacity);

                    string windowTitle = builder.ToString();

                    if (windowTitle.Contains(game, StringComparison.OrdinalIgnoreCase))
                    {
                        running = true;
                    }

                    return true;
                }, IntPtr.Zero);
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Running: {running}");
            return running;
        }
    }
}
