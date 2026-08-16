// Copyright © - 12/08/2026 - Toby Hunter
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace LiveScreensaver
{
    public partial class App : Application
    {
        /// <summary>
        /// Runs the application code.
        /// </summary>
        private void ApplicationStartup(
            object sender,
            StartupEventArgs e)
        {
            string arg = e.Args.Length > 0 ? e.Args[0].ToLower()
                .TrimStart('-', '/') : "c";

            if (arg.StartsWith('s'))
            {
                string videoFolder = GetVideoFolder();

                if (string.IsNullOrEmpty(videoFolder) || !Directory.Exists(videoFolder))
                {
                    MessageBox.Show(
                        "No video folder configured or folder not found.\n\nRight-click the screensaver and choose 'Settings' to set your video folder.",
                        "Live Screensaver",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    Shutdown();

                    return;
                }

                string[] files = Directory.GetFiles(videoFolder, "*.mp4");

                if (files.Length == 0)
                {
                    MessageBox.Show(
                        $"No .mp4 files found in:\n{videoFolder}",
                        "Live Screensaver",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    Shutdown();

                    return;
                }

                string startupScript = GetScript();

                if (!string.IsNullOrEmpty(startupScript) && File.Exists(startupScript))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -WindowStyle Hidden -File \"{startupScript}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }

                ScreensaverWindow window = new(files);

                window.Show();
            }

            else if (arg.StartsWith('c'))
            {
                SettingsWindow settings = new();
                settings.ShowDialog();

                Shutdown();
            }

            else if (arg.StartsWith('p'))
            {
                Shutdown();
            }

            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Returns the folder for the videos from the registry
        /// </summary>
        public static string GetVideoFolder()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\LiveScreensaver");
            return key?.GetValue("VideoFolder") as string ?? "";
        }

        /// <summary>
        /// Adds a registry entry to store the video folder.
        /// </summary>
        public static void SetVideoFolder(string path)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\LiveScreensaver");
            key.SetValue("VideoFolder", path);
        }

        /// <summary>
        /// Returns the script path from the registry.
        /// </summary>
        public static string GetScript()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\LiveScreensaver");
            return key?.GetValue("Script") as string ?? "";
        }

        /// <summary>
        /// Adds a registry entry to store the script path.
        /// </summary>
        public static void SetScript(string path)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\LiveScreensaver");
            key.SetValue("Script", path);
        }
    }
}
