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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.splitterBottom = new System.Windows.Forms.Splitter();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressLabel = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblInfoLabel = new System.Windows.Forms.Label();
            this.lblTask = new System.Windows.Forms.Label();
            this.lblTaskLabel = new System.Windows.Forms.Label();
            this.gridApps = new System.Windows.Forms.DataGridView();
            this.colIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTyp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colExcluded = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLauncher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctxmAppActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miActivate = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeactivate = new System.Windows.Forms.ToolStripMenuItem();
            this.miInstall = new System.Windows.Forms.ToolStripMenuItem();
            this.miReinstall = new System.Windows.Forms.ToolStripMenuItem();
            this.miUpgrade = new System.Windows.Forms.ToolStripMenuItem();
            this.miUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.miDownloadResource = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteResource = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadList = new Mastersign.Bench.Dashboard.DownloadList();
            this.menuStrip = new Mastersign.Bench.Dashboard.ImmediateMenuStrip();
            this.tsmSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInstallAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReinstallAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUpgradeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUninstallAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDownloadAllResources = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteAllResources = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCustomApps = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditActivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditDeactivationList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlwaysShowDownloads = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridApps)).BeginInit();
            this.ctxmAppActions.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // splitterBottom
            // 
            this.splitterBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterBottom.Location = new System.Drawing.Point(0, 243);
            this.splitterBottom.Name = "splitterBottom";
            this.splitterBottom.Size = new System.Drawing.Size(860, 5);
            this.splitterBottom.TabIndex = 0;
            this.splitterBottom.TabStop = false;
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.panelStatus.Controls.Add(this.progressBar);
            this.panelStatus.Controls.Add(this.lblProgressLabel);
            this.panelStatus.Controls.Add(this.lblInfo);
            this.panelStatus.Controls.Add(this.lblInfoLabel);
            this.panelStatus.Controls.Add(this.lblTask);
            this.panelStatus.Controls.Add(this.lblTaskLabel);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatus.Location = new System.Drawing.Point(0, 24);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(860, 74);
            this.panelStatus.TabIndex = 7;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(72, 50);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(776, 13);
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
            this.lblInfo.Size = new System.Drawing.Size(779, 13);
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
            this.lblTask.Size = new System.Drawing.Size(779, 13);
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
            this.lblTaskLabel.Size = new System.Drawing.Size(31, 13);
            this.lblTaskLabel.TabIndex = 0;
            this.lblTaskLabel.Text = "Task:";
            // 
            // gridApps
            // 
            this.gridApps.AllowUserToAddRows = false;
            this.gridApps.AllowUserToDeleteRows = false;
            this.gridApps.AllowUserToResizeRows = false;
            this.gridApps.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridApps.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridApps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridApps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIcon,
            this.colIndex,
            this.colID,
            this.colTyp,
            this.colActivated,
            this.colExcluded,
            this.colVersion,
            this.colLauncher,
            this.colStatus,
            this.colComment});
            this.gridApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridApps.Location = new System.Drawing.Point(0, 98);
            this.gridApps.MultiSelect = false;
            this.gridApps.Name = "gridApps";
            this.gridApps.RowHeadersVisible = false;
            this.gridApps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridApps.ShowEditingIcon = false;
            this.gridApps.Size = new System.Drawing.Size(860, 145);
            this.gridApps.TabIndex = 8;
            this.gridApps.VirtualMode = true;
            this.gridApps.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridApps_ColumnHeaderMouseClick);
            this.gridApps.RowContextMenuStripNeeded += new System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler(this.gridApps_RowContextMenuStripNeeded);
            // 
            // colIcon
            // 
            this.colIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colIcon.DataPropertyName = "StatusIcon";
            this.colIcon.Frozen = true;
            this.colIcon.HeaderText = "";
            this.colIcon.Name = "colIcon";
            this.colIcon.ReadOnly = true;
            this.colIcon.Width = 32;
            // 
            // colIndex
            // 
            this.colIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colIndex.DataPropertyName = "Index";
            this.colIndex.Frozen = true;
            this.colIndex.HeaderText = "Order";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.ToolTipText = "The index number from the app registry.";
            this.colIndex.Width = 62;
            // 
            // colID
            // 
            this.colID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colID.DataPropertyName = "ID";
            this.colID.Frozen = true;
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.ToolTipText = "The ID of the app.";
            this.colID.Width = 43;
            // 
            // colTyp
            // 
            this.colTyp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTyp.DataPropertyName = "Typ";
            this.colTyp.HeaderText = "Typ";
            this.colTyp.Name = "colTyp";
            this.colTyp.ReadOnly = true;
            this.colTyp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTyp.ToolTipText = "The typ of the app.";
            this.colTyp.Width = 48;
            // 
            // colActivated
            // 
            this.colActivated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colActivated.DataPropertyName = "IsActive";
            this.colActivated.FalseValue = "inactive";
            this.colActivated.HeaderText = "Active";
            this.colActivated.IndeterminateValue = "implicit";
            this.colActivated.Name = "colActivated";
            this.colActivated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colActivated.ThreeState = true;
            this.colActivated.ToolTipText = "States whether the app is activated by the user or not.";
            this.colActivated.TrueValue = "active";
            this.colActivated.Width = 62;
            // 
            // colExcluded
            // 
            this.colExcluded.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colExcluded.DataPropertyName = "IsDeactivated";
            this.colExcluded.HeaderText = "Deactivated";
            this.colExcluded.Name = "colExcluded";
            this.colExcluded.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colExcluded.ToolTipText = "States whether the app is deactivated by the user.";
            this.colExcluded.Width = 92;
            // 
            // colVersion
            // 
            this.colVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.HeaderText = "Version";
            this.colVersion.Name = "colVersion";
            this.colVersion.ReadOnly = true;
            this.colVersion.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colVersion.Width = 70;
            // 
            // colLauncher
            // 
            this.colLauncher.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colLauncher.DataPropertyName = "Launcher";
            this.colLauncher.HeaderText = "Launcher";
            this.colLauncher.Name = "colLauncher";
            this.colLauncher.ReadOnly = true;
            this.colLauncher.Width = 79;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colStatus.DataPropertyName = "ShortStatus";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 64;
            // 
            // colComment
            // 
            this.colComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colComment.DataPropertyName = "LongStatus";
            this.colComment.HeaderText = "Comment";
            this.colComment.Name = "colComment";
            this.colComment.ReadOnly = true;
            // 
            // ctxmAppActions
            // 
            this.ctxmAppActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miActivate,
            this.miDeactivate,
            toolStripSeparator2,
            this.miInstall,
            this.miReinstall,
            this.miUpgrade,
            this.miUninstall,
            toolStripSeparator1,
            this.miDownloadResource,
            this.miDeleteResource});
            this.ctxmAppActions.Name = "ctxMenuAppActions";
            this.ctxmAppActions.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ctxmAppActions.Size = new System.Drawing.Size(180, 192);
            // 
            // miActivate
            // 
            this.miActivate.Name = "miActivate";
            this.miActivate.Size = new System.Drawing.Size(179, 22);
            this.miActivate.Text = "&Activate";
            this.miActivate.Click += new System.EventHandler(this.ActivateAppHandler);
            // 
            // miDeactivate
            // 
            this.miDeactivate.Name = "miDeactivate";
            this.miDeactivate.Size = new System.Drawing.Size(179, 22);
            this.miDeactivate.Text = "&Deactivate";
            this.miDeactivate.Click += new System.EventHandler(this.DeactivateAppHandler);
            // 
            // miInstall
            // 
            this.miInstall.Name = "miInstall";
            this.miInstall.Size = new System.Drawing.Size(179, 22);
            this.miInstall.Text = "&Install";
            this.miInstall.Click += new System.EventHandler(this.InstallAppHandler);
            // 
            // miReinstall
            // 
            this.miReinstall.Name = "miReinstall";
            this.miReinstall.Size = new System.Drawing.Size(179, 22);
            this.miReinstall.Text = "R&einstall";
            this.miReinstall.Click += new System.EventHandler(this.ReinstallAppHandler);
            // 
            // miUpgrade
            // 
            this.miUpgrade.Name = "miUpgrade";
            this.miUpgrade.Size = new System.Drawing.Size(179, 22);
            this.miUpgrade.Text = "&Upgrade";
            this.miUpgrade.Click += new System.EventHandler(this.UpgradeAppHandler);
            // 
            // miUninstall
            // 
            this.miUninstall.Name = "miUninstall";
            this.miUninstall.Size = new System.Drawing.Size(179, 22);
            this.miUninstall.Text = "U&ninstall";
            this.miUninstall.Click += new System.EventHandler(this.UninstallAppHandler);
            // 
            // miDownloadResource
            // 
            this.miDownloadResource.Name = "miDownloadResource";
            this.miDownloadResource.Size = new System.Drawing.Size(179, 22);
            this.miDownloadResource.Text = "Do&wnload Resource";
            this.miDownloadResource.Click += new System.EventHandler(this.DownloadAppResourceHandler);
            // 
            // miDeleteResource
            // 
            this.miDeleteResource.Name = "miDeleteResource";
            this.miDeleteResource.Size = new System.Drawing.Size(179, 22);
            this.miDeleteResource.Text = "&Delete Resource";
            this.miDeleteResource.Click += new System.EventHandler(this.DeleteAppResourceHandler);
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
            this.downloadList.Size = new System.Drawing.Size(860, 153);
            this.downloadList.TabIndex = 6;
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
            this.menuStrip.Size = new System.Drawing.Size(860, 24);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "Menu";
            // 
            // tsmSetup
            // 
            this.tsmSetup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiInstallAll,
            this.tsmiReinstallAll,
            this.tsmiUpgradeAll,
            this.tsmiUninstallAll,
            toolStripSeparator3,
            this.tsmiDownloadAllResources,
            this.tsmiDeleteAllResources});
            this.tsmSetup.Name = "tsmSetup";
            this.tsmSetup.Size = new System.Drawing.Size(49, 20);
            this.tsmSetup.Text = "&Setup";
            // 
            // tsmiInstallAll
            // 
            this.tsmiInstallAll.Name = "tsmiInstallAll";
            this.tsmiInstallAll.Size = new System.Drawing.Size(184, 22);
            this.tsmiInstallAll.Text = "&Install Apps";
            this.tsmiInstallAll.Click += new System.EventHandler(this.InstallAllHandler);
            // 
            // tsmiReinstallAll
            // 
            this.tsmiReinstallAll.Name = "tsmiReinstallAll";
            this.tsmiReinstallAll.Size = new System.Drawing.Size(184, 22);
            this.tsmiReinstallAll.Text = "R&einstall Apps";
            this.tsmiReinstallAll.Click += new System.EventHandler(this.ReinstallAllHandler);
            // 
            // tsmiUpgradeAll
            // 
            this.tsmiUpgradeAll.Name = "tsmiUpgradeAll";
            this.tsmiUpgradeAll.Size = new System.Drawing.Size(184, 22);
            this.tsmiUpgradeAll.Text = "&Upgrade Apps";
            this.tsmiUpgradeAll.Click += new System.EventHandler(this.UpgradeAllHandler);
            // 
            // tsmiUninstallAll
            // 
            this.tsmiUninstallAll.Name = "tsmiUninstallAll";
            this.tsmiUninstallAll.Size = new System.Drawing.Size(184, 22);
            this.tsmiUninstallAll.Text = "U&ninstall Apps";
            this.tsmiUninstallAll.Click += new System.EventHandler(this.UninstallAllHandler);
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
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 401);
            this.Controls.Add(this.gridApps);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.splitterBottom);
            this.Controls.Add(this.downloadList);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupForm";
            this.Text = "Bench - Setup";
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridApps)).EndInit();
            this.ctxmAppActions.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitterBottom;
        private ImmediateMenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmSetup;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownloadAllResources;
        private System.Windows.Forms.ToolStripMenuItem tsmiInstallAll;
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
        private System.Windows.Forms.DataGridView gridApps;
        private System.Windows.Forms.ContextMenuStrip ctxmAppActions;
        private System.Windows.Forms.ToolStripMenuItem miInstall;
        private System.Windows.Forms.ToolStripMenuItem miReinstall;
        private System.Windows.Forms.ToolStripMenuItem miUpgrade;
        private System.Windows.Forms.ToolStripMenuItem miDownloadResource;
        private System.Windows.Forms.ToolStripMenuItem miDeleteResource;
        private System.Windows.Forms.ToolStripMenuItem miUninstall;
        private System.Windows.Forms.ToolStripMenuItem miActivate;
        private System.Windows.Forms.ToolStripMenuItem miDeactivate;
        private System.Windows.Forms.ToolStripMenuItem tsmiReinstallAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiUpgradeAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiUninstallAll;
        private System.Windows.Forms.DataGridViewImageColumn colIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTyp;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActivated;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colExcluded;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLauncher;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
    }
}