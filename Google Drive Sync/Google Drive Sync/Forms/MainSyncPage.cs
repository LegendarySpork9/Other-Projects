using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    public partial class MainSyncPage : Form
    {
        private readonly ApplicationService AppService = new ApplicationService();
        private List<FileModel> Files = new List<FileModel>();
        private DataGridViewRow CurrentRow;
        private bool TablePopulated = false;
        private List<DataGridViewRow> RowsToUpdate = new List<DataGridViewRow>();

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

        // Increases the value of the pogress bar.
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

        // Populates the file information grid.
        private async void BTNCompareClick(object sender, EventArgs e)
        {
            BTNCompare.Enabled = false;
            BTNSync.Enabled = false;
            BTNSync.Text = "Sync 0 File(s)";
            DGVFileInformation.Rows.Clear();
            RowsToUpdate.Clear();
            TBCDocumentChanges.Visible = false;

            PRBLoading.Value = 0;
            PBLoading.Image = Properties.Resources.LoadingSpinner;
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

                else
                {
                    if (file.Changes[0].OldValue == "Not Uploaded")
                    {
                        DataGridViewComboBoxCell dropDown = DGVFileInformation.Rows[DGVFileInformation.Rows.Count - 1].Cells[6] as DataGridViewComboBoxCell;

                        dropDown.Items.Remove("Down Stream");
                    }

                    else if (file.Changes[0].OldValue == "Not Downloaded")
                    {
                        DataGridViewComboBoxCell dropDown = DGVFileInformation.Rows[DGVFileInformation.Rows.Count - 1].Cells[6] as DataGridViewComboBoxCell;

                        dropDown.Items.Remove("Up Stream");
                    }
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

            PBUp.Enabled = true;
            PBDown.Enabled = true;
            BTNCompare.Enabled = true;
        }

        // Triggers the process of syncing the chosen files.
        private async void BTNSyncClick(object sender, EventArgs e)
        {
            BTNSync.Enabled = false;
            BTNCompare.Enabled = false;
            DGVFileInformation.ReadOnly = true;
            TBCDocumentChanges.Visible = false;

            PRBLoading.Value = 0;
            PBLoading.Image = Properties.Resources.LoadingSpinner;
            bool hasErrored = false;

            List<FileModel> uploadFiles = new List<FileModel>();
            List<FileModel> downloadFiles = new List<FileModel>();

            foreach (DataGridViewRow row in RowsToUpdate)
            {
                if (row.Cells[6].Value.ToString() == "Up Stream")
                {
                    uploadFiles.Add(Files.Find(c => c.Name == row.Cells[0].Value.ToString() && c.Type == row.Cells[1].Value.ToString()));
                }

                else
                {
                    downloadFiles.Add(Files.Find(c => c.Name == row.Cells[0].Value.ToString() && c.Type == row.Cells[1].Value.ToString()));
                }
            }

            await Task.Run(() =>
            {
                hasErrored = AppService.SyncChanges(uploadFiles, downloadFiles);
            });

            foreach (FileModel file in uploadFiles)
            {
                file.Changes.Clear();
            }

            foreach (FileModel file in downloadFiles)
            {
                file.Changes.Clear();
            }

            DGVFileInformation.CellValueChanged -= DGVFileInformationCellValue;

            foreach (DataGridViewRow row in RowsToUpdate)
            {
                int index = DGVFileInformation.Rows.IndexOf(row);

                DGVFileInformation.Rows[index].Cells[5].Value = "False";
                DGVFileInformation.Rows[index].Cells[6].Value = null;
                DGVFileInformation.Rows[index].Cells[7].Value = null;
            }

            DGVFileInformation.CellValueChanged += DGVFileInformationCellValue;

            PRBLoading.Value = 100;

            if (hasErrored)
            {
                PBLoading.Image = Properties.Resources.Cross;
            }

            else
            {
                PBLoading.Image = Properties.Resources.Tick;
            }

            BTNSync.Text = "Sync 0 File(s)";
            RowsToUpdate.Clear();
            DGVFileInformation.ReadOnly = false;
            BTNCompare.Enabled = true;
        }

        // Loads the file information into the information box.
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

        // Checks merge type and update cells to see if a merge should be done.
        private void DGVFileInformationCellValue(object sender, DataGridViewCellEventArgs e)
        {
            if (TablePopulated)
            {
                DataGridViewRow row = DGVFileInformation.Rows[e.RowIndex];

                if (row.Cells[6].Value != null && (row.Cells[7].Value != null && bool.Parse(row.Cells[7].Value.ToString())))
                {
                    if (!RowsToUpdate.Contains(row))
                    {
                        int previousUpdateFileCount = RowsToUpdate.Count;

                        RowsToUpdate.Add(row);

                        BTNSync.Text = BTNSync.Text.Replace(previousUpdateFileCount.ToString(), RowsToUpdate.Count.ToString());

                        if (RowsToUpdate.Count > 0 && !BTNSync.Enabled)
                        {
                            BTNSync.Enabled = true;
                        }

                        if (RowsToUpdate.Count == 0 && BTNSync.Enabled)
                        {
                            BTNSync.Enabled = false;
                        }
                    }

                    else
                    {
                        RowsToUpdate.Remove(row);
                        RowsToUpdate.Add(row);
                    }
                }

                else
                {
                    if (RowsToUpdate.Contains(row))
                    {
                        int previousUpdateFileCount = RowsToUpdate.Count;

                        RowsToUpdate.Remove(row);

                        BTNSync.Text = BTNSync.Text.Replace(previousUpdateFileCount.ToString(), RowsToUpdate.Count.ToString());

                        if (RowsToUpdate.Count > 0 && !BTNSync.Enabled)
                        {
                            BTNSync.Enabled = true;
                        }

                        if (RowsToUpdate.Count == 0 && BTNSync.Enabled)
                        {
                            BTNSync.Enabled = false;
                        }
                    }
                }
            }
        }

        // Sets all files with changes to be synced up stream.
        private void SyncUp(object sender, EventArgs e)
        {
            RowsToUpdate.Clear();

            foreach (DataGridViewRow row in DGVFileInformation.Rows)
            {
                DataGridViewComboBoxCell dropDown = row.Cells[6] as DataGridViewComboBoxCell;

                if (bool.Parse(row.Cells[5].Value.ToString()) && dropDown.Items.Contains("Up Stream"))
                {
                    row.Cells[6].Value = "Up Stream";
                    row.Cells[7].Value = true;
                }                
            }
        }

        // Sets all files with changes to be synced down stream.
        private void SyncDown(object sender, EventArgs e)
        {
            RowsToUpdate.Clear();

            foreach (DataGridViewRow row in DGVFileInformation.Rows)
            {
                DataGridViewComboBoxCell dropDown = row.Cells[6] as DataGridViewComboBoxCell;

                if (bool.Parse(row.Cells[5].Value.ToString()) && dropDown.Items.Contains("Down Stream"))
                {
                    row.Cells[6].Value = "Down Stream";
                    row.Cells[7].Value = true;
                }
            }
        }

        // Starts the auto sync timer.
        private void CBAutoSyncChecked(object sender, EventArgs e)
        {
            if (CBAutoSync.Checked)
            {
                BTNCompare.Enabled = false;
                PBUp.Enabled = false;
                PBDown.Enabled = false;

                TMAutoSync.Enabled = true;
                TMAutoSync.Start();
            }

            else
            {
                TMAutoSync.Enabled = false;
                BTNCompare.Enabled = true;
            }
        }

        // Performs the auto sync process.
        private async void TMAutoSyncElapsedAsync(object sender, EventArgs e)
        {
            TMAutoSync.Stop();

            FileFunction _fileFunction = new FileFunction();
            GoogleDriveFunction _googleDriveFunction = new GoogleDriveFunction();

            PRBLoading.Value = 0;
            PBLoading.Image = Properties.Resources.LoadingSpinner;
            bool hasErrored = false;

            await Task.Run(() =>
            {
                (Files, hasErrored) = AppService.CheckUpdates();
            });

            PRBLoading.Value = 0;

            List<FileModel> uploadFiles = new List<FileModel>();
            List<FileModel> downloadFiles = new List<FileModel>();

            foreach (FileModel file in Files)
            {
                if ((file.Id.Contains(",") && !_fileFunction.IsFileLocked(new FileInfo(_googleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right")))) || (file.Id.Contains(":\\") && !_fileFunction.IsFileLocked(new FileInfo(file.Id))))
                {
                    if (file.Changes.Count > 0)
                    {
                        ChangeModel modifiedChange = file.Changes.Find(c => c.Field == "Last Modified");
                        ChangeModel pathChange = file.Changes.Find(c => c.Field == "Path");

                        if (pathChange == null && modifiedChange != null)
                        {
                            if (modifiedChange.Stream == "Up")
                            {
                                downloadFiles.Add(file);
                            }

                            else
                            {
                                uploadFiles.Add(file);
                            }
                        }
                    }
                }
            }

            await Task.Run(() =>
            {
                hasErrored = AppService.SyncChanges(uploadFiles, downloadFiles);
            });

            if (hasErrored)
            {
                PBLoading.Image = Properties.Resources.Cross;
            }

            else
            {
                PBLoading.Image = Properties.Resources.Tick;
            }

            PRBLoading.Value = 100;
            TMAutoSync.Start();
        }

        // Logs the closing message.
        private void Exit(object sender, FormClosedEventArgs e)
        {
            LoggerService _logger = new LoggerService();

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
        }
    }
}
