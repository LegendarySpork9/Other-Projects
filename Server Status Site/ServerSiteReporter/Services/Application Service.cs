using ServerSiteCommon.Converters;
using ServerSiteCommon.Functions;
using ServerSiteCommon.Models;
using ServerSiteCommon.Models.API;
using ServerSiteCommon.Models.Data;
using ServerSiteCommon.Services;
using ServerSiteReporter.Models;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServerSiteReporter.Services
{
    public class ApplicationService
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

        private LoggerService Logger = new();
        private readonly APIService APIService;
        private SharedSettingsModel SharedSettings;
        private Timer RefreshTimer;
        private DateTime NextElapse;

        // Sets the class's global variables.
        public ApplicationService(SharedSettingsModel sharedSettings)
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
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configuring Application Service");

            RefreshTimer = new()
            {
                AutoReset = false
            };
            RefreshTimer.Elapsed += (sender, e) => TimerElapsed(sender, e);

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Timer Duration: {SharedSettings.RefreshTime} minutes");

            APIService.SetLogger(Logger);

            Logger.LogMessage(StandardValues.LoggerValues.Info, "Configured Application Service");
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
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Running Event Register");

            List<ServerModel> servers = APIService.GetServers();

            foreach (string game in AppSettingsModel.Games)
            {
                string[] gameParts = game.Split('_');
                gameParts[1] = gameParts[1].Replace("(", "").Replace(")", "");

                ServerModel? server = servers.Find(c => c.HostName == AppSettingsModel.HostName && c.Game == gameParts[0] && c.GameVersion == gameParts[1]);

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Registering Events for {server.HostName ?? StandardValues.MissingValues.HostName} - {server.Game ?? StandardValues.MissingValues.Game} ({server.GameVersion ?? StandardValues.MissingValues.GameVersion})");

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
                                HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                Game = server.Game ?? StandardValues.MissingValues.Game,
                                GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
                            }
                        };

                        if (APIService.RegisterServerEvent(newStatus))
                        {
                            Logger.LogMessage(StandardValues.LoggerValues.Debug, "Server Event Registered");
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
                                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                    Game = server.Game ?? StandardValues.MissingValues.Game,
                                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
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
                                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                    Game = server.Game ?? StandardValues.MissingValues.Game,
                                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
                                }
                            };

                            if (APIService.RegisterServerEvent(newStatus))
                            {
                                Logger.LogMessage(StandardValues.LoggerValues.Debug, "Server Event Registered");
                            }
                        }
                    }

                    if (component == "Connection")
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"IP Address: {server.Connection.IPAddress}");

                        string pingStatus = PingAddress(server.Connection.IPAddress, server.Connection.Port);

                        if (pingStatus == "Success")
                        {
                            APIStatusModel newStatus = new()
                            {
                                Component = "Connection Status",
                                Status = "Online",
                                Server = new APIRelatedServerModel
                                {
                                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                    Game = server.Game ?? StandardValues.MissingValues.Game,
                                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
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
                                Component = "Connection Status",
                                Status = "Offline",
                                Server = new APIRelatedServerModel
                                {
                                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                    Game = server.Game ?? StandardValues.MissingValues.Game,
                                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
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
                                Component = "Connection Status",
                                Status = "Unknown",
                                Server = new APIRelatedServerModel
                                {
                                    HostName = server.HostName ?? StandardValues.MissingValues.HostName,
                                    Game = server.Game ?? StandardValues.MissingValues.Game,
                                    GameVersion = server.GameVersion ?? StandardValues.MissingValues.GameVersion
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

        // Tries to ping a given IP address.
        private string PingAddress(string ipAddress, int port)
        {
            string response = string.Empty;

            try
            {
                using (TcpClient client = new())
                {
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured TCP Client");
                    Logger.LogMessage(StandardValues.LoggerValues.Debug, "Pinging Address");

                    IAsyncResult result = client.BeginConnect(ipAddress, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Connection Status: {success}");

                    if (success)
                    {
                        response = "Success";
                    }

                    else
                    {
                        response = "Failed";
                    }

                    client.EndConnect(result);
                }
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return response;
        }

        // Checks if a process with the given name is running.
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
