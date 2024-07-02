namespace Book_Reader
{
    partial class Book_Reader
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Book_Reader));
            this.BTBack = new System.Windows.Forms.Button();
            this.BTNext = new System.Windows.Forms.Button();
            this.LBIgnore = new System.Windows.Forms.Label();
            this.RTBChapterText = new System.Windows.Forms.RichTextBox();
            this.TMCycleImage = new System.Windows.Forms.Timer(this.components);
            this.PBTTS = new System.Windows.Forms.PictureBox();
            this.PBOpen = new System.Windows.Forms.PictureBox();
            this.PBImages = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PBTTS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBImages)).BeginInit();
            this.SuspendLayout();
            // 
            // BTBack
            // 
            this.BTBack.Enabled = false;
            this.BTBack.Location = new System.Drawing.Point(852, 415);
            this.BTBack.Name = "BTBack";
            this.BTBack.Size = new System.Drawing.Size(75, 23);
            this.BTBack.TabIndex = 5;
            this.BTBack.Text = "Back";
            this.BTBack.UseVisualStyleBackColor = true;
            this.BTBack.Click += new System.EventHandler(this.Back);
            // 
            // BTNext
            // 
            this.BTNext.Enabled = false;
            this.BTNext.Location = new System.Drawing.Point(933, 415);
            this.BTNext.Name = "BTNext";
            this.BTNext.Size = new System.Drawing.Size(75, 23);
            this.BTNext.TabIndex = 6;
            this.BTNext.Text = "Next";
            this.BTNext.UseVisualStyleBackColor = true;
            this.BTNext.Click += new System.EventHandler(this.Next);
            // 
            // LBIgnore
            // 
            this.LBIgnore.AutoSize = true;
            this.LBIgnore.Location = new System.Drawing.Point(605, 420);
            this.LBIgnore.Name = "LBIgnore";
            this.LBIgnore.Size = new System.Drawing.Size(37, 13);
            this.LBIgnore.TabIndex = 8;
            this.LBIgnore.Text = "Ignore";
            this.LBIgnore.Visible = false;
            // 
            // RTBChapterText
            // 
            this.RTBChapterText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RTBChapterText.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTBChapterText.Location = new System.Drawing.Point(333, 12);
            this.RTBChapterText.MaxLength = 32767;
            this.RTBChapterText.Name = "RTBChapterText";
            this.RTBChapterText.ReadOnly = true;
            this.RTBChapterText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RTBChapterText.Size = new System.Drawing.Size(675, 397);
            this.RTBChapterText.TabIndex = 9;
            this.RTBChapterText.Text = "";
            this.RTBChapterText.Click += new System.EventHandler(this.Deselect);
            // 
            // TMCycleImage
            // 
            this.TMCycleImage.Interval = 1;
            this.TMCycleImage.Tick += new System.EventHandler(this.CycleImage);
            // 
            // PBTTS
            // 
            this.PBTTS.Image = global::Book_Reader.Properties.Resources.Mute;
            this.PBTTS.Location = new System.Drawing.Point(370, 415);
            this.PBTTS.Name = "PBTTS";
            this.PBTTS.Size = new System.Drawing.Size(31, 23);
            this.PBTTS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PBTTS.TabIndex = 10;
            this.PBTTS.TabStop = false;
            this.PBTTS.Click += new System.EventHandler(this.TTS);
            // 
            // PBOpen
            // 
            this.PBOpen.Image = global::Book_Reader.Properties.Resources.open_folder;
            this.PBOpen.Location = new System.Drawing.Point(333, 415);
            this.PBOpen.Name = "PBOpen";
            this.PBOpen.Size = new System.Drawing.Size(31, 23);
            this.PBOpen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PBOpen.TabIndex = 7;
            this.PBOpen.TabStop = false;
            this.PBOpen.Click += new System.EventHandler(this.Open);
            // 
            // PBImages
            // 
            this.PBImages.Location = new System.Drawing.Point(12, 12);
            this.PBImages.Name = "PBImages";
            this.PBImages.Size = new System.Drawing.Size(315, 426);
            this.PBImages.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PBImages.TabIndex = 2;
            this.PBImages.TabStop = false;
            // 
            // Book_Reader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 450);
            this.Controls.Add(this.PBTTS);
            this.Controls.Add(this.RTBChapterText);
            this.Controls.Add(this.LBIgnore);
            this.Controls.Add(this.PBOpen);
            this.Controls.Add(this.BTNext);
            this.Controls.Add(this.BTBack);
            this.Controls.Add(this.PBImages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1036, 489);
            this.MinimumSize = new System.Drawing.Size(1036, 489);
            this.Name = "Book_Reader";
            this.Text = "Book Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing);
            ((System.ComponentModel.ISupportInitialize)(this.PBTTS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBImages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PBImages;
        private System.Windows.Forms.Button BTBack;
        private System.Windows.Forms.Button BTNext;
        private System.Windows.Forms.PictureBox PBOpen;
        private System.Windows.Forms.Label LBIgnore;
        private System.Windows.Forms.RichTextBox RTBChapterText;
        private System.Windows.Forms.Timer TMCycleImage;
        private System.Windows.Forms.PictureBox PBTTS;
    }
}

