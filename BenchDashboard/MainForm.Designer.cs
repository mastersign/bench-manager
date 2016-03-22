namespace Mastersign.Bench.Dashboard
{
    partial class MainForm
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
            this.btnDownload = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.appLauncherList = new Mastersign.Bench.Dashboard.AppLauncherControl();
            this.downloadList = new Mastersign.Bench.Dashboard.DownloadList();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(309, 62);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(16, 21);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(28, 13);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Info";
            // 
            // appLauncherControl1
            // 
            this.appLauncherList.AppIndex = null;
            this.appLauncherList.Location = new System.Drawing.Point(12, 91);
            this.appLauncherList.Name = "appLauncherControl1";
            this.appLauncherList.Size = new System.Drawing.Size(291, 229);
            this.appLauncherList.TabIndex = 3;
            // 
            // downloadList
            // 
            this.downloadList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadList.AutoScroll = true;
            this.downloadList.BackColor = System.Drawing.SystemColors.Window;
            this.downloadList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.downloadList.Downloader = null;
            this.downloadList.Location = new System.Drawing.Point(309, 91);
            this.downloadList.Name = "downloadList";
            this.downloadList.Size = new System.Drawing.Size(441, 229);
            this.downloadList.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 332);
            this.Controls.Add(this.appLauncherList);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.downloadList);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.Text = "Bench";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DownloadList downloadList;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label lblInfo;
        private AppLauncherControl appLauncherList;
    }
}

