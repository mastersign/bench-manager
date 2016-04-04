namespace Mastersign.Bench.Dashboard
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.splitterBottom = new System.Windows.Forms.Splitter();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomApps = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditActivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditDeactivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlwaysShowDownloads = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadList = new Mastersign.Bench.Dashboard.DownloadList();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitterBottom
            // 
            this.splitterBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterBottom.Location = new System.Drawing.Point(0, 243);
            this.splitterBottom.Name = "splitterBottom";
            this.splitterBottom.Size = new System.Drawing.Size(472, 5);
            this.splitterBottom.TabIndex = 0;
            this.splitterBottom.TabStop = false;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSetup,
            this.tsmEdit,
            this.tsmView});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(472, 24);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "Menu";
            // 
            // tsmSetup
            // 
            this.tsmSetup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDownload,
            this.tsmiSetup});
            this.tsmSetup.Name = "tsmSetup";
            this.tsmSetup.Size = new System.Drawing.Size(49, 20);
            this.tsmSetup.Text = "&Setup";
            // 
            // tsmiDownload
            // 
            this.tsmiDownload.Name = "tsmiDownload";
            this.tsmiDownload.Size = new System.Drawing.Size(184, 22);
            this.tsmiDownload.Text = "Download Resources";
            this.tsmiDownload.Click += new System.EventHandler(this.DownloadAllHandler);
            // 
            // tsmiSetup
            // 
            this.tsmiSetup.Name = "tsmiSetup";
            this.tsmiSetup.Size = new System.Drawing.Size(184, 22);
            this.tsmiSetup.Text = "Install Apps";
            // 
            // tsmEdit
            // 
            this.tsmEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditCustomConfig,
            this.tsmiEditCustomApps,
            this.tsmiEditActivationList,
            this.tsmiEditDeactivationList});
            this.tsmEdit.Name = "tsmEdit";
            this.tsmEdit.Size = new System.Drawing.Size(116, 20);
            this.tsmEdit.Text = "&Edit Configuration";
            // 
            // tsmiEditCustomConfig
            // 
            this.tsmiEditCustomConfig.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.config;
            this.tsmiEditCustomConfig.Name = "tsmiEditCustomConfig";
            this.tsmiEditCustomConfig.Size = new System.Drawing.Size(193, 22);
            this.tsmiEditCustomConfig.Text = "&Custom Configuration";
            this.tsmiEditCustomConfig.Click += new System.EventHandler(this.EditCustomConfigHandler);
            // 
            // tsmiEditCustomApps
            // 
            this.tsmiEditCustomApps.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.apps;
            this.tsmiEditCustomApps.Name = "tsmiEditCustomApps";
            this.tsmiEditCustomApps.Size = new System.Drawing.Size(193, 22);
            this.tsmiEditCustomApps.Text = "C&ustom Apps";
            this.tsmiEditCustomApps.Click += new System.EventHandler(this.EditCustomAppsHandler);
            // 
            // tsmiEditActivationList
            // 
            this.tsmiEditActivationList.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.include;
            this.tsmiEditActivationList.Name = "tsmiEditActivationList";
            this.tsmiEditActivationList.Size = new System.Drawing.Size(193, 22);
            this.tsmiEditActivationList.Text = "&Activated Apps";
            this.tsmiEditActivationList.Click += new System.EventHandler(this.ActivationListHandler);
            // 
            // tsmiEditDeactivationList
            // 
            this.tsmiEditDeactivationList.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.exclude;
            this.tsmiEditDeactivationList.Name = "tsmiEditDeactivationList";
            this.tsmiEditDeactivationList.Size = new System.Drawing.Size(193, 22);
            this.tsmiEditDeactivationList.Text = "&Deactivated Apps";
            this.tsmiEditDeactivationList.Click += new System.EventHandler(this.DeactivationListHandler);
            // 
            // tsmView
            // 
            this.tsmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAlwaysShowDownloads});
            this.tsmView.Name = "tsmView";
            this.tsmView.Size = new System.Drawing.Size(44, 20);
            this.tsmView.Text = "&View";
            // 
            // tsmiAlwaysShowDownloads
            // 
            this.tsmiAlwaysShowDownloads.CheckOnClick = true;
            this.tsmiAlwaysShowDownloads.Name = "tsmiAlwaysShowDownloads";
            this.tsmiAlwaysShowDownloads.Size = new System.Drawing.Size(205, 22);
            this.tsmiAlwaysShowDownloads.Text = "&Always Show Downloads";
            this.tsmiAlwaysShowDownloads.CheckedChanged += new System.EventHandler(this.AlwaysShowDownloadsCheckedChanged);
            // 
            // downloadList
            // 
            this.downloadList.AutoScroll = true;
            this.downloadList.BackColor = System.Drawing.SystemColors.Window;
            this.downloadList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.downloadList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.downloadList.Downloader = null;
            this.downloadList.Location = new System.Drawing.Point(0, 248);
            this.downloadList.Name = "downloadList";
            this.downloadList.Size = new System.Drawing.Size(472, 153);
            this.downloadList.TabIndex = 6;
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 401);
            this.Controls.Add(this.splitterBottom);
            this.Controls.Add(this.downloadList);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupForm";
            this.Text = "Bench - Setup";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitterBottom;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmSetup;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownload;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetup;
        private System.Windows.Forms.ToolStripMenuItem tsmEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCustomConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCustomApps;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditActivationList;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditDeactivationList;
        private DownloadList downloadList;
        private System.Windows.Forms.ToolStripMenuItem tsmView;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlwaysShowDownloads;
    }
}