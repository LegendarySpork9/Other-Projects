namespace GoogleDriveSync
{
    partial class MainSyncPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainSyncPage));
            this.DGVFileInformation = new System.Windows.Forms.DataGridView();
            this.FName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DCreated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HChanges = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.UFile = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TBCDocumentChanges = new System.Windows.Forms.TabControl();
            this.TBPDocumentChanges = new System.Windows.Forms.TabPage();
            this.PRBLoading = new System.Windows.Forms.ProgressBar();
            this.PBLoading = new System.Windows.Forms.PictureBox();
            this.BTNCompare = new System.Windows.Forms.Button();
            this.BTNSync = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGVFileInformation)).BeginInit();
            this.TBCDocumentChanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PBLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // DGVFileInformation
            // 
            this.DGVFileInformation.AllowUserToAddRows = false;
            this.DGVFileInformation.AllowUserToDeleteRows = false;
            this.DGVFileInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVFileInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FName,
            this.FType,
            this.FPath,
            this.DCreated,
            this.LModified,
            this.HChanges,
            this.MType,
            this.UFile});
            this.DGVFileInformation.Location = new System.Drawing.Point(0, 23);
            this.DGVFileInformation.Name = "DGVFileInformation";
            this.DGVFileInformation.Size = new System.Drawing.Size(844, 450);
            this.DGVFileInformation.TabIndex = 0;
            // 
            // FName
            // 
            this.FName.HeaderText = "File Name";
            this.FName.Name = "FName";
            this.FName.ReadOnly = true;
            // 
            // FType
            // 
            this.FType.HeaderText = "File Type";
            this.FType.Name = "FType";
            this.FType.ReadOnly = true;
            // 
            // FPath
            // 
            this.FPath.HeaderText = "File Path";
            this.FPath.Name = "FPath";
            this.FPath.ReadOnly = true;
            // 
            // DCreated
            // 
            this.DCreated.HeaderText = "Date Created";
            this.DCreated.Name = "DCreated";
            this.DCreated.ReadOnly = true;
            // 
            // LModified
            // 
            this.LModified.HeaderText = "Last Modified";
            this.LModified.Name = "LModified";
            this.LModified.ReadOnly = true;
            // 
            // HChanges
            // 
            this.HChanges.HeaderText = "Has Changes";
            this.HChanges.Name = "HChanges";
            this.HChanges.ReadOnly = true;
            // 
            // MType
            // 
            this.MType.HeaderText = "Merge Type";
            this.MType.Items.AddRange(new object[] {
            "Up Stream",
            "Down Stream"});
            this.MType.Name = "MType";
            // 
            // UFile
            // 
            this.UFile.HeaderText = "Update";
            this.UFile.Name = "UFile";
            // 
            // TBCDocumentChanges
            // 
            this.TBCDocumentChanges.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TBCDocumentChanges.Controls.Add(this.TBPDocumentChanges);
            this.TBCDocumentChanges.ItemSize = new System.Drawing.Size(0, 1);
            this.TBCDocumentChanges.Location = new System.Drawing.Point(847, 23);
            this.TBCDocumentChanges.Margin = new System.Windows.Forms.Padding(0);
            this.TBCDocumentChanges.Name = "TBCDocumentChanges";
            this.TBCDocumentChanges.Padding = new System.Drawing.Point(0, 0);
            this.TBCDocumentChanges.SelectedIndex = 0;
            this.TBCDocumentChanges.Size = new System.Drawing.Size(400, 450);
            this.TBCDocumentChanges.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TBCDocumentChanges.TabIndex = 1;
            // 
            // TBPDocumentChanges
            // 
            this.TBPDocumentChanges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TBPDocumentChanges.Location = new System.Drawing.Point(4, 5);
            this.TBPDocumentChanges.Margin = new System.Windows.Forms.Padding(0);
            this.TBPDocumentChanges.Name = "TBPDocumentChanges";
            this.TBPDocumentChanges.Size = new System.Drawing.Size(392, 441);
            this.TBPDocumentChanges.TabIndex = 0;
            this.TBPDocumentChanges.UseVisualStyleBackColor = true;
            // 
            // PRBLoading
            // 
            this.PRBLoading.Location = new System.Drawing.Point(432, 0);
            this.PRBLoading.Name = "PRBLoading";
            this.PRBLoading.Size = new System.Drawing.Size(400, 23);
            this.PRBLoading.TabIndex = 0;
            // 
            // PBLoading
            // 
            this.PBLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PBLoading.Location = new System.Drawing.Point(838, 0);
            this.PBLoading.Name = "PBLoading";
            this.PBLoading.Size = new System.Drawing.Size(23, 23);
            this.PBLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PBLoading.TabIndex = 2;
            this.PBLoading.TabStop = false;
            // 
            // BTNCompare
            // 
            this.BTNCompare.Location = new System.Drawing.Point(544, 473);
            this.BTNCompare.Name = "BTNCompare";
            this.BTNCompare.Size = new System.Drawing.Size(86, 23);
            this.BTNCompare.TabIndex = 3;
            this.BTNCompare.Text = "Compare";
            this.BTNCompare.UseVisualStyleBackColor = true;
            this.BTNCompare.Click += new System.EventHandler(this.BTNCompareClick);
            // 
            // BTNSync
            // 
            this.BTNSync.Enabled = false;
            this.BTNSync.Location = new System.Drawing.Point(635, 473);
            this.BTNSync.Name = "BTNSync";
            this.BTNSync.Size = new System.Drawing.Size(86, 23);
            this.BTNSync.TabIndex = 4;
            this.BTNSync.Text = "Sync 0 Files";
            this.BTNSync.UseVisualStyleBackColor = true;
            // 
            // MainSyncPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 496);
            this.Controls.Add(this.BTNSync);
            this.Controls.Add(this.BTNCompare);
            this.Controls.Add(this.PBLoading);
            this.Controls.Add(this.PRBLoading);
            this.Controls.Add(this.DGVFileInformation);
            this.Controls.Add(this.TBCDocumentChanges);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainSyncPage";
            this.Text = "Google Drive Sync";
            ((System.ComponentModel.ISupportInitialize)(this.DGVFileInformation)).EndInit();
            this.TBCDocumentChanges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PBLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGVFileInformation;
        private System.Windows.Forms.TabControl TBCDocumentChanges;
        private System.Windows.Forms.TabPage TBPDocumentChanges;
        private System.Windows.Forms.ProgressBar PRBLoading;
        private System.Windows.Forms.PictureBox PBLoading;
        private System.Windows.Forms.Button BTNCompare;
        private System.Windows.Forms.DataGridViewTextBoxColumn FName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn DCreated;
        private System.Windows.Forms.DataGridViewTextBoxColumn LModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn HChanges;
        private System.Windows.Forms.DataGridViewComboBoxColumn MType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UFile;
        private System.Windows.Forms.Button BTNSync;
    }
}

