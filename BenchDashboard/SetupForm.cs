using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public partial class SetupForm : Form
    {
        private readonly Core core;

        public SetupForm(Core core)
        {
            this.core = core;
            InitializeComponent();
            InitializeDownloadList();
        }

        private void InitializeDownloadList()
        {
            downloadList.Downloader = core.Downloader;
            core.Downloader.IsWorkingChanged += DownloaderIsWorkingChangedHandler;
            IsDownloadListVisible = false;
        }

        private void UpdateProgressBar(float progress)
        {
            progressBar.Value = progressBar.Minimum + (int)((progressBar.Maximum - progressBar.Minimum) * progress);
        }

        private void DownloaderIsWorkingChangedHandler(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((EventHandler)DownloaderIsWorkingChangedHandler);
                return;
            }
            UpdateDownloadListVisibility();
        }

        private void UpdateDownloadListVisibility()
        {
            IsDownloadListVisible = core.Downloader.IsWorking || tsmiAlwaysShowDownloads.Checked;
        }

        protected bool IsDownloadListVisible
        {
            get { return downloadList.Visible; }
            set
            {
                SuspendLayout();
                splitterBottom.Visible = value;
                downloadList.Visible = value;
                ResumeLayout();
            }
        }

        private void AlwaysShowDownloadsCheckedChanged(object sender, EventArgs e)
        {
            UpdateDownloadListVisibility();
        }

        private void EditTextFile(string name, string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show(
                    "File of " + name + " not found."
                    + Environment.NewLine + Environment.NewLine
                    + path,
                    "Edit " + name + " ...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            core.UI.EditTextFile(path);
        }

        private void EditCustomConfigHandler(object sender, EventArgs e)
        {
            EditTextFile("Custom Configuration",
                core.Config.GetStringValue(PropertyKeys.CustomConfigFile));
        }

        private void EditCustomAppsHandler(object sender, EventArgs e)
        {
            EditTextFile("Custom App Index",
                core.Config.GetStringValue(PropertyKeys.CustomAppIndexFile));
        }

        private void ActivationListHandler(object sender, EventArgs e)
        {
            EditTextFile("App Activation",
                core.Config.GetStringValue(PropertyKeys.AppActivationFile));
        }

        private void DeactivationListHandler(object sender, EventArgs e)
        {
            EditTextFile("App Deactivation",
                core.Config.GetStringValue(PropertyKeys.AppDeactivationFile));
        }

        private void ProgressInfoHandler(string info, bool error, float progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke((ProgressCallback)ProgressInfoHandler, info, error, progress);
                return;
            }
            lblInfo.Text = info;
            UpdateProgressBar(progress);
        }

        private void DownloadAllHandler(object sender, EventArgs e)
        {
            lblTask.Text = "Download app resources";
            core.DownloadAppResources(ProgressInfoHandler);
        }

        private void DeleteAllResourcesHandler(object sender, EventArgs e)
        {
            lblTask.Text = "Delete app resources";
            core.DeleteAppResources(ProgressInfoHandler);
        }

        private void InstallAllHandler(object sender, EventArgs e)
        {
            lblTask.Text = "Install apps";
            core.InstallApps(ProgressInfoHandler);
        }
    }
}
