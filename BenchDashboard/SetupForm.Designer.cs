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
            this.menuStrip = new Mastersign.Bench.Dashboard.ImmediateMenuStrip();
            this.tsmSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDownloadAllResources = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteAllResources = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomApps = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditActivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditDeactivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlwaysShowDownloads = new System.Windows.Forms.ToolStripMenuItem();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressLabel = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblInfoLabel = new System.Windows.Forms.Label();
            this.lblTask = new System.Windows.Forms.Label();
            this.lblTaskLabel = new System.Windows.Forms.Label();
            this.downloadList = new Mastersign.Bench.Dashboard.DownloadList();
            this.menuStrip.SuspendLayout();
            this.panelStatus.SuspendLayout();
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
            this.tsmiDownloadAllResources,
            this.tsmiDeleteAllResources,
            this.tsmiSetup});
            this.tsmSetup.Name = "tsmSetup";
            this.tsmSetup.Size = new System.Drawing.Size(49, 20);
            this.tsmSetup.Text = "&Setup";
            // 
            // tsmiDownloadAllResources
            // 
            this.tsmiDownloadAllResources.Name = "tsmiDownloadAllResources";
            this.tsmiDownloadAllResources.Size = new System.Drawing.Size(184, 22);
            this.tsmiDownloadAllResources.Text = "Do&wnload Resources";
            this.tsmiDownloadAllResources.Click += new System.EventHandler(this.DownloadAllHandler);
            // 
            // tsmiDeleteAllResources
            // 
            this.tsmiDeleteAllResources.Name = "tsmiDeleteAllResources";
            this.tsmiDeleteAllResources.Size = new System.Drawing.Size(184, 22);
            this.tsmiDeleteAllResources.Text = "&Delete Resources";
            this.tsmiDeleteAllResources.Click += new System.EventHandler(this.DeleteAllResourcesHandler);
            // 
            // tsmiSetup
            // 
            this.tsmiSetup.Name = "tsmiSetup";
            this.tsmiSetup.Size = new System.Drawing.Size(184, 22);
            this.tsmiSetup.Text = "&Install Apps";
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
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.SystemColors.Window;
            this.panelStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStatus.Controls.Add(this.progressBar);
            this.panelStatus.Controls.Add(this.lblProgressLabel);
            this.panelStatus.Controls.Add(this.lblInfo);
            this.panelStatus.Controls.Add(this.lblInfoLabel);
            this.panelStatus.Controls.Add(this.lblTask);
            this.panelStatus.Controls.Add(this.lblTaskLabel);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatus.Location = new System.Drawing.Point(0, 24);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(472, 74);
            this.panelStatus.TabIndex = 7;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(72, 50);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(384, 13);
            this.progressBar.TabIndex = 5;
            // 
            // lblProgressLabel
            // 
            this.lblProgressLabel.AutoSize = true;
            this.lblProgressLabel.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblProgressLabel.Location = new System.Drawing.Point(12, 50);
            this.lblProgressLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lblProgressLabel.Name = "lblProgressLabel";
            this.lblProgressLabel.Size = new System.Drawing.Size(54, 13);
            this.lblProgressLabel.TabIndex = 4;
            this.lblProgressLabel.Text = "Progress:";
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(69, 29);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(387, 13);
            this.lblInfo.TabIndex = 3;
            // 
            // lblInfoLabel
            // 
            this.lblInfoLabel.AutoSize = true;
            this.lblInfoLabel.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblInfoLabel.Location = new System.Drawing.Point(12, 29);
            this.lblInfoLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lblInfoLabel.Name = "lblInfoLabel";
            this.lblInfoLabel.Size = new System.Drawing.Size(31, 13);
            this.lblInfoLabel.TabIndex = 2;
            this.lblInfoLabel.Text = "Info:";
            // 
            // lblTask
            // 
            this.lblTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTask.Location = new System.Drawing.Point(69, 8);
            this.lblTask.Name = "lblTask";
            this.lblTask.Size = new System.Drawing.Size(387, 13);
            this.lblTask.TabIndex = 1;
            this.lblTask.Text = "none";
            // 
            // lblTaskLabel
            // 
            this.lblTaskLabel.AutoSize = true;
            this.lblTaskLabel.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblTaskLabel.Location = new System.Drawing.Point(12, 8);
            this.lblTaskLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lblTaskLabel.Name = "lblTaskLabel";
            this.lblTaskLabel.Size = new System.Drawing.Size(32, 13);
            this.lblTaskLabel.TabIndex = 0;
            this.lblTaskLabel.Text = "Task:";
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
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.splitterBottom);
            this.Controls.Add(this.downloadList);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupForm";
            this.Text = "Bench - Setup";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitterBottom;
        private ImmediateMenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmSetup;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownloadAllResources;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetup;
        private System.Windows.Forms.ToolStripMenuItem tsmEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCustomConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCustomApps;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditActivationList;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditDeactivationList;
        private DownloadList downloadList;
        private System.Windows.Forms.ToolStripMenuItem tsmView;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlwaysShowDownloads;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblTask;
        private System.Windows.Forms.Label lblTaskLabel;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblInfoLabel;
        private System.Windows.Forms.Label lblProgressLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteAllResources;
    }
}