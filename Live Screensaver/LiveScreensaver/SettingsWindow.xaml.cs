// Copyright © - 12/08/2026 - Toby Hunter
using System.Windows;
using Microsoft.Win32;

namespace LiveScreensaver
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            FolderPath.Text = App.GetVideoFolder();
            ScriptPath.Text = App.GetScript();
        }
        
        /// <summary>
        /// Opens a dialog to select a folder.
        /// </summary>
        private void BrowseClick(
            object sender,
            RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title = "Select Video Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                FolderPath.Text = dialog.FolderName;
            }
        }

        /// <summary>
        /// Opens a dialog to select a script.
        /// </summary>
        private void BrowseScriptClick(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Title = "Select Startup Script",
                Filter = "PowerShell Scripts (*.ps1)|*.ps1"
            };

            if (dialog.ShowDialog() == true)
            {
                ScriptPath.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Triggers the saving of the settings.
        /// </summary>
        private void OKClick(
            object sender,
            RoutedEventArgs e)
        {
            App.SetVideoFolder(FolderPath.Text);
            App.SetScript(ScriptPath.Text);

            Close();
        }

        /// <summary>
        /// Closes the settings window.
        /// </summary>
        private void CancelClick(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}
