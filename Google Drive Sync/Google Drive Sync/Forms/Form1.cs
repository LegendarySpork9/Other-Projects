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

namespace GoogleDriveSync
{
    public partial class MainSyncPage : Form
    {
        private readonly ApplicationService AppService = new ApplicationService();

        public MainSyncPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PBLoading.Image = Properties.Resources.LoadingSpinner;
            //PRBLoading.Value = 20;
            //DGVFileInformation.Rows.Add("Test", "docx", "books", "22/04/2025", "22/04/2025", "No");
        }

        private async void BTNCompareClick(object sender, EventArgs e)
        {
            PBLoading.Image = Properties.Resources.LoadingSpinner;

            await Task.Run(() =>
            {
                AppService.CheckUpdates(PRBLoading);
            });
        }
    }
}
