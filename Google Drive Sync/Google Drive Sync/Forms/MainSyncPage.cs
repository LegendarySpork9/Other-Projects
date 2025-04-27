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
        }

        private void DGVFileInformationRowState(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            if (CurrentRow != e.Row)
            {
                CurrentRow = e.Row;
            }
        }
    }
}
