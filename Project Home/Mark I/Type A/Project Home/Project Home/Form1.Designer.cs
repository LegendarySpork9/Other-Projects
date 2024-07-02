namespace Project_Home
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.TBHome = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TBHome
            // 
            this.TBHome.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TBHome.ForeColor = System.Drawing.Color.Lime;
            this.TBHome.Location = new System.Drawing.Point(-2, -2);
            this.TBHome.Multiline = true;
            this.TBHome.Name = "TBHome";
            this.TBHome.ReadOnly = true;
            this.TBHome.Size = new System.Drawing.Size(807, 455);
            this.TBHome.TabIndex = 0;
            this.TBHome.TextChanged += new System.EventHandler(this.TBHome_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TBHome);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Home";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TBHome;
    }
}

