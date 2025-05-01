using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private bool TablePopulated = false;
        private List<DataGridViewRow> RowsToUpload = new List<DataGridViewRow>();
        private int UpdateFileCount = 0;

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
            PRBLoading.Value = 0;
            PBLoading.Image = Properties.Resources.LoadingSpinner;
            DGVFileInformation.Rows.Clear();
            bool hasErrored = false;

            await Task.Run(() =>
            {
                (Files, hasErrored) = AppService.CheckUpdates();
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
                
                if (hasChanges == "False")
                {
                    DGVFileInformation.Rows[DGVFileInformation.Rows.Count - 1].Cells[6].ReadOnly = true;
                    DGVFileInformation.Rows[DGVFileInformation.Rows.Count - 1].Cells[7].ReadOnly = true;
                }
            }

            TablePopulated = true;
            PRBLoading.Value = 100;

            if (hasErrored)
            {
                PBLoading.Image = Properties.Resources.Cross;
            }

            else
            {
                PBLoading.Image = Properties.Resources.Tick;
            }

            BTNCompare.Enabled = true;
        }

        private async void BTNSyncClick(object sender, EventArgs e)
        {
            BTNSync.Enabled = false;
            BTNCompare.Enabled = false;
            PRBLoading.Value = 0;
            PBLoading.Image = Properties.Resources.LoadingSpinner;
            bool hasErrored = false;

            List<FileModel> files = new List<FileModel>();

            foreach(DataGridViewRow row in RowsToUpload)
            {
                files.Add(Files.Find(c => c.Name == row.Cells[0].Value.ToString() && c.Type == row.Cells[1].Value.ToString()));
            }

            await Task.Run(() =>
            {
                hasErrored = AppService.SyncChanges(files);
            });

            PRBLoading.Value = 100;

            if (hasErrored)
            {
                PBLoading.Image = Properties.Resources.Cross;
            }

            else
            {
                PBLoading.Image = Properties.Resources.Tick;
            }

            BTNCompare.Enabled = true;
            BTNSync.Enabled = true;
        }

        private void DGVFileInformationRowState(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            if (CurrentRow != e.Row)
            {
                DGVChanges.Rows.Clear();
                CurrentRow = e.Row;

                FileModel file = Files.Find(c => c.Name == CurrentRow.Cells[0].Value.ToString() && c.Type == CurrentRow.Cells[1].Value.ToString());

                if (file.Id.Contains(","))
                {
                    TBId.Text = file.Id.Remove(file.Id.IndexOf(','));
                    TBName.Text = file.Name;
                    TBType.Text = file.Type;
                    TBPathIds.Text = file.PathIds;
                    TBGDPath.Text = file.Path.Remove(file.Path.IndexOf(','));
                    TBLPath.Text = file.Path.Remove(0, file.Path.IndexOf(',') + 1);
                    TBHidden.Text = file.Hidden.ToString();
                    TBCreated.Text = file.Created.ToString();
                    TBModified.Text = file.LastModified.ToString();

                    foreach (ChangeModel change in file.Changes)
                    {
                        DGVChanges.Rows.Add(change.Field, change.OldValue, change.NewValue, change.Stream);
                    }
                }

                else
                {
                    if (file.Id.Contains(@":\"))
                    {
                        TBId.Text = "";
                        TBName.Text = file.Name;
                        TBType.Text = file.Type;
                        TBPathIds.Text = file.PathIds;
                        TBGDPath.Text = "";
                        TBLPath.Text = file.Path;
                        TBHidden.Text = file.Hidden.ToString();
                        TBCreated.Text = file.Created.ToString();
                        TBModified.Text = file.LastModified.ToString();

                        foreach (ChangeModel change in file.Changes)
                        {
                            DGVChanges.Rows.Add(change.Field, change.OldValue, change.NewValue, change.Stream);
                        }
                    }

                    else
                    {
                        TBId.Text = file.Id;
                        TBName.Text = file.Name;
                        TBType.Text = file.Type;
                        TBPathIds.Text = file.PathIds;
                        TBGDPath.Text = file.Path;
                        TBHidden.Text = file.Hidden.ToString();
                        TBCreated.Text = file.Created.ToString();
                        TBModified.Text = file.LastModified.ToString();

                        foreach (ChangeModel change in file.Changes)
                        {
                            DGVChanges.Rows.Add(change.Field, change.OldValue, change.NewValue, change.Stream);
                        }
                    }
                }

                TBCDocumentChanges.Visible = true;
            }
        }

        private void DGVFileInformationCellValue(object sender, DataGridViewCellEventArgs e)
        {
            if (TablePopulated)
            {
                DataGridViewRow row = DGVFileInformation.Rows[e.RowIndex];

                if (row.Cells[6].Value != null && (row.Cells[7].Value != null && bool.Parse(row.Cells[7].Value.ToString())))
                {
                    RowsToUpload.Add(row);

                    int previousUpdateFileCount = UpdateFileCount;
                    UpdateFileCount++;

                    BTNSync.Text = BTNSync.Text.Replace(char.Parse(previousUpdateFileCount.ToString()), char.Parse(UpdateFileCount.ToString()));

                    if (UpdateFileCount > 0 && !BTNSync.Enabled)
                    {
                        BTNSync.Enabled = true;
                    }

                    if (UpdateFileCount == 0 && BTNSync.Enabled)
                    {
                        BTNSync.Enabled = false;
                    }
                }

                else
                {
                    if (RowsToUpload.Contains(row))
                    {
                        RowsToUpload.Remove(row);

                        int previousUpdateFileCount = UpdateFileCount;
                        UpdateFileCount--;

                        BTNSync.Text = BTNSync.Text.Replace(char.Parse(previousUpdateFileCount.ToString()), char.Parse(UpdateFileCount.ToString()));

                        if (UpdateFileCount > 0 && !BTNSync.Enabled)
                        {
                            BTNSync.Enabled = true;
                        }

                        if (UpdateFileCount == 0 && BTNSync.Enabled)
                        {
                            BTNSync.Enabled = false;
                        }
                    }
                }
            }
        }
    }
}
