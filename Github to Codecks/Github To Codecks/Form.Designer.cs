namespace Github_To_Codecks
{
    partial class Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmbIssue = new System.Windows.Forms.ComboBox();
            this.cmbRepo = new System.Windows.Forms.ComboBox();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.ptbRepo = new System.Windows.Forms.PictureBox();
            this.ptbIssue = new System.Windows.Forms.PictureBox();
            this.ptbProject = new System.Windows.Forms.PictureBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.ptbConfirm = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ptbRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbIssue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbConfirm)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(32, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(316, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Hunter Industries: Github to Codecks";
            // 
            // cmbIssue
            // 
            this.cmbIssue.Enabled = false;
            this.cmbIssue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbIssue.FormattingEnabled = true;
            this.cmbIssue.Location = new System.Drawing.Point(36, 159);
            this.cmbIssue.Name = "cmbIssue";
            this.cmbIssue.Size = new System.Drawing.Size(264, 24);
            this.cmbIssue.TabIndex = 1;
            this.cmbIssue.Text = "Choose an Issue";
            this.cmbIssue.SelectedIndexChanged += new System.EventHandler(this.IssueSelected);
            // 
            // cmbRepo
            // 
            this.cmbRepo.Enabled = false;
            this.cmbRepo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRepo.FormattingEnabled = true;
            this.cmbRepo.Location = new System.Drawing.Point(36, 95);
            this.cmbRepo.Name = "cmbRepo";
            this.cmbRepo.Size = new System.Drawing.Size(264, 24);
            this.cmbRepo.TabIndex = 2;
            this.cmbRepo.Text = "Choose a Repository";
            this.cmbRepo.SelectedIndexChanged += new System.EventHandler(this.RepoSelected);
            // 
            // cmbProject
            // 
            this.cmbProject.Enabled = false;
            this.cmbProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.Location = new System.Drawing.Point(36, 227);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(264, 24);
            this.cmbProject.TabIndex = 3;
            this.cmbProject.Text = "Choose a Codecks Project";
            this.cmbProject.SelectedIndexChanged += new System.EventHandler(this.ProjectSelected);
            // 
            // ptbRepo
            // 
            this.ptbRepo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptbRepo.Location = new System.Drawing.Point(317, 95);
            this.ptbRepo.Name = "ptbRepo";
            this.ptbRepo.Size = new System.Drawing.Size(32, 24);
            this.ptbRepo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptbRepo.TabIndex = 4;
            this.ptbRepo.TabStop = false;
            // 
            // ptbIssue
            // 
            this.ptbIssue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptbIssue.Location = new System.Drawing.Point(317, 159);
            this.ptbIssue.Name = "ptbIssue";
            this.ptbIssue.Size = new System.Drawing.Size(32, 24);
            this.ptbIssue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptbIssue.TabIndex = 5;
            this.ptbIssue.TabStop = false;
            // 
            // ptbProject
            // 
            this.ptbProject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptbProject.Location = new System.Drawing.Point(317, 227);
            this.ptbProject.Name = "ptbProject";
            this.ptbProject.Size = new System.Drawing.Size(32, 24);
            this.ptbProject.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptbProject.TabIndex = 6;
            this.ptbProject.TabStop = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Enabled = false;
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Location = new System.Drawing.Point(124, 295);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(152, 26);
            this.btnConfirm.TabIndex = 7;
            this.btnConfirm.Text = "Confirm and Send";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.SendClick);
            // 
            // ptbConfirm
            // 
            this.ptbConfirm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptbConfirm.Location = new System.Drawing.Point(184, 327);
            this.ptbConfirm.Name = "ptbConfirm";
            this.ptbConfirm.Size = new System.Drawing.Size(32, 24);
            this.ptbConfirm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptbConfirm.TabIndex = 8;
            this.ptbConfirm.TabStop = false;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.ptbConfirm);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.ptbProject);
            this.Controls.Add(this.ptbIssue);
            this.Controls.Add(this.ptbRepo);
            this.Controls.Add(this.cmbProject);
            this.Controls.Add(this.cmbRepo);
            this.Controls.Add(this.cmbIssue);
            this.Controls.Add(this.lblTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "Form";
            this.Text = "Github to Codecks";
            this.Shown += new System.EventHandler(this.LoadRepos);
            ((System.ComponentModel.ISupportInitialize)(this.ptbRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbIssue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbConfirm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cmbIssue;
        private System.Windows.Forms.ComboBox cmbRepo;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.PictureBox ptbRepo;
        private System.Windows.Forms.PictureBox ptbIssue;
        private System.Windows.Forms.PictureBox ptbProject;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.PictureBox ptbConfirm;
    }
}

