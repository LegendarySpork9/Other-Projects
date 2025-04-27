using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GoogleDriveSync
{
    public partial class MainSyncPage : Form
    {
        private readonly ApplicationService AppService = new ApplicationService();
        private List<FileModel> Files = new List<FileModel>();
        private DataGridViewRow CurrentRow;

        public MainSyncPage()
        {
            InitializeComponent();

            LoggerService _logger = new LoggerService();

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Google Drive Folder: {AppSettingsModel.DriveFolder}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Local Folder: {AppSettingsModel.LocalFolder}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Ignore Folder(s): {string.Join(",", AppSettingsModel.IgnoreFolders)}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Ignore File(s): {string.Join(",", AppSettingsModel.IgnoreFiles)}");

            AppService.ProgressChanged += ProgressChanged;
        }

        private void ProgressChanged(int value)
        {
            if (PRBLoading.InvokeRequired)
            {
                PRBLoading.Invoke(new Action(() => PRBLoading.Value = value));
            }

            else
            {
                PRBLoading.Value = value;
            }
        }

        private async void BTNCompareClick(object sender, EventArgs e)
        {
            BTNCompare.Enabled = false;
            PBLoading.Image = Properties.Resources.LoadingSpinner;

            await Task.Run(() =>
            {
                Files = AppService.CheckUpdates();
            });

            foreach (FileModel file in Files)
            {
                string path = "";
                string hasChanges = "False";

                if (file.Path.Contains(","))
                {
                    path = file.Path.Substring(0, file.Path.IndexOf(','));
                }

                else
                {
                    path = file.Path;
                }

                if (file.Changes.Count > 0)
                {
                    hasChanges = "True";
                }

                DGVFileInformation.Rows.Add(file.Name, file.Type, path, file.Created, file.LastModified, hasChanges);
            }

            PRBLoading.Value = 100;
            PBLoading.Image = Properties.Resources.Tick;
            BTNCompare.Enabled = true;
        }

        private void DGVFileInformationRowState(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            if (CurrentRow != e.Row)
            {
                DGVChanges.Rows.Clear();
                CurrentRow = e.Row;

                FileModel file = Files.Find(c => c.Name == CurrentRow.Cells[0].Value.ToString() && c.Type == CurrentRow.Cells[1].Value.ToString());
                string[] fileIdSplit = file.Id.Split(',');

                TBId.Text = fileIdSplit[0];
                TBName.Text = file.Name;
                TBType.Text = file.Type;
                TBPathIds.Text = file.PathIds;
                TBGDPath.Text = file.Path.Remove(file.Path.IndexOf(','));
                TBLPath.Text = fileIdSplit[1] ?? fileIdSplit[0];
                TBHidden.Text = file.Hidden.ToString();
                TBCreated.Text = file.Created.ToString();
                TBModified.Text = file.LastModified.ToString();

                foreach(ChangeModel change in file.Changes)
                {
                    DGVChanges.Rows.Add(change.Field, change.OldValue, change.NewValue, change.Stream);
                }

                TBCDocumentChanges.Visible = true;
            }
        }
    }
}
