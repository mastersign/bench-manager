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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslRootPathLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRootPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslAppCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslAppCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnShellPowerShell = new System.Windows.Forms.Button();
            this.btnShellCmd = new System.Windows.Forms.Button();
            this.btnSetup = new System.Windows.Forms.Button();
            this.appLauncherList = new Mastersign.Bench.Dashboard.AppLauncherControl();
            this.statusStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslRootPathLabel,
            this.tsslRootPath,
            this.tsslAppCountLabel,
            this.tsslAppCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 361);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(416, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsslRootPathLabel
            // 
            this.tsslRootPathLabel.Name = "tsslRootPathLabel";
            this.tsslRootPathLabel.Size = new System.Drawing.Size(62, 17);
            this.tsslRootPathLabel.Text = "Root Path:";
            // 
            // tsslRootPath
            // 
            this.tsslRootPath.IsLink = true;
            this.tsslRootPath.Name = "tsslRootPath";
            this.tsslRootPath.Size = new System.Drawing.Size(47, 17);
            this.tsslRootPath.Text = "<Path>";
            this.tsslRootPath.Click += new System.EventHandler(this.RootPathClickHandler);
            // 
            // tsslAppCountLabel
            // 
            this.tsslAppCountLabel.Name = "tsslAppCountLabel";
            this.tsslAppCountLabel.Size = new System.Drawing.Size(73, 17);
            this.tsslAppCountLabel.Text = "Active Apps:";
            // 
            // tsslAppCount
            // 
            this.tsslAppCount.Name = "tsslAppCount";
            this.tsslAppCount.Size = new System.Drawing.Size(56, 17);
            this.tsslAppCount.Text = "<Count>";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnShellPowerShell);
            this.panelTop.Controls.Add(this.btnShellCmd);
            this.panelTop.Controls.Add(this.btnSetup);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(416, 29);
            this.panelTop.TabIndex = 5;
            // 
            // btnShellPowerShell
            // 
            this.btnShellPowerShell.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.missing_app_16;
            this.btnShellPowerShell.Location = new System.Drawing.Point(73, 3);
            this.btnShellPowerShell.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnShellPowerShell.Name = "btnShellPowerShell";
            this.btnShellPowerShell.Size = new System.Drawing.Size(97, 23);
            this.btnShellPowerShell.TabIndex = 2;
            this.btnShellPowerShell.Text = "PowerShell";
            this.btnShellPowerShell.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnShellPowerShell.UseVisualStyleBackColor = true;
            this.btnShellPowerShell.Click += new System.EventHandler(this.ShellPowerShellHandler);
            // 
            // btnShellCmd
            // 
            this.btnShellCmd.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.missing_app_16;
            this.btnShellCmd.Location = new System.Drawing.Point(3, 3);
            this.btnShellCmd.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnShellCmd.Name = "btnShellCmd";
            this.btnShellCmd.Size = new System.Drawing.Size(67, 23);
            this.btnShellCmd.TabIndex = 1;
            this.btnShellCmd.Text = "CMD";
            this.btnShellCmd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnShellCmd.UseVisualStyleBackColor = true;
            this.btnShellCmd.Click += new System.EventHandler(this.ShellCmdHandler);
            // 
            // btnSetup
            // 
            this.btnSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetup.Image = global::Mastersign.Bench.Dashboard.Properties.Resources.setup_16;
            this.btnSetup.Location = new System.Drawing.Point(338, 3);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(75, 23);
            this.btnSetup.TabIndex = 0;
            this.btnSetup.Text = "Setup";
            this.btnSetup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler(this.SetupHandler);
            // 
            // appLauncherList
            // 
            this.appLauncherList.AppIndex = null;
            this.appLauncherList.Core = null;
            this.appLauncherList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appLauncherList.Location = new System.Drawing.Point(0, 29);
            this.appLauncherList.Name = "appLauncherList";
            this.appLauncherList.Size = new System.Drawing.Size(416, 332);
            this.appLauncherList.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 383);
            this.Controls.Add(this.appLauncherList);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Bench";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AppLauncherControl appLauncherList;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslRootPathLabel;
        private System.Windows.Forms.ToolStripStatusLabel tsslRootPath;
        private System.Windows.Forms.ToolStripStatusLabel tsslAppCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel tsslAppCount;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnShellCmd;
        private System.Windows.Forms.Button btnShellPowerShell;
    }
}

