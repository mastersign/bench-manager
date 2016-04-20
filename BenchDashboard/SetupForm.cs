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

        private DataGridViewColumn sortedColumn;
        private ListSortDirection sortDirection;
        private AppFacade contextApp;

        public SetupForm(Core core)
        {
            this.core = core;
            core.ConfigReloaded += ConfigReloadedHandler;
            InitializeComponent();
            gridApps.AutoGenerateColumns = false;
            InitializeDownloadList();
            InitializeAppList();
        }

        private void ConfigReloadedHandler(object sender, EventArgs e)
        {
            var selectedRow = gridApps.SelectedRows.Count > 0 ? gridApps.SelectedRows[0].Index : -1;
            InitializeAppList();
            if (gridApps.Rows.Count >= selectedRow + 1)
            {
                gridApps.Rows[selectedRow].Selected = true;
            }
        }

        private void InitializeDownloadList()
        {
            downloadList.Downloader = core.Downloader;
            core.Downloader.IsWorkingChanged += DownloaderIsWorkingChangedHandler;
            IsDownloadListVisible = false;
        }

        private void InitializeAppList()
        {
            var list = new List<AppWrapper>();
            var cnt = 0;
            foreach (var app in core.Config.Apps)
            {
                cnt++;
                list.Add(new AppWrapper(app, cnt));
            }
            gridApps.DataSource = new SortedBindingList<AppWrapper>(list);
            if (sortedColumn != null)
            {
                gridApps.Sort(sortedColumn, sortDirection);
            }
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

        private void AnnounceTask(string label)
        {
            lblTask.Text = label;
        }

        private void DownloadAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Download app resources");
            core.DownloadAppResources(ProgressInfoHandler);
        }

        private void DeleteAllResourcesHandler(object sender, EventArgs e)
        {
            AnnounceTask("Delete app resources");
            core.DeleteAppResources(ProgressInfoHandler);
        }

        private void InstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Install apps");
            core.InstallApps(ProgressInfoHandler);
        }

        private void UninstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Uninstall apps");
            core.UninstallApps(ProgressInfoHandler);
        }

        private void ReinstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Reinstall apps");
            MessageBox.Show("Not implemented yet.");
        }

        private void UpgradeAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade apps");
            MessageBox.Show("Not implemented yet.");
        }

        private void ActivateAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Activate app " + contextApp.ID);
            contextApp = null;
        }

        private void DeactivateAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Deactivate app " + contextApp.ID);
            contextApp = null;
        }

        private void InstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Install app " + contextApp.ID);
            core.InstallApp(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void ReinstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Reinstall app " + contextApp.ID);
            contextApp = null;
        }

        private void UpgradeAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade app " + contextApp.ID);
            contextApp = null;
        }

        private void DownloadAppResourceHandler(object sender, EventArgs e)
        {
            AnnounceTask("Download app resource for " + contextApp.ID);
            core.DownloadAppResource(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void DeleteAppResourceHandler(object sender, EventArgs e)
        {
            AnnounceTask("Delete app resource app for " + contextApp.ID);
            core.DeleteAppResource(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void UninstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Uninstall app " + contextApp.ID);
            core.UninstallApp(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void gridApps_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var col = gridApps.Columns[e.ColumnIndex];
            if (col == sortedColumn)
            {
                switch (sortDirection)
                {
                    case ListSortDirection.Ascending:
                        sortDirection = ListSortDirection.Descending;
                        break;
                    case ListSortDirection.Descending:
                        sortDirection = ListSortDirection.Ascending;
                        break;
                }
            }
            else
            {
                sortedColumn = col;
                sortDirection = ListSortDirection.Ascending;
            }
            gridApps.Sort(sortedColumn, sortDirection);
        }

        private void gridApps_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
        {
            var row = gridApps.Rows[e.RowIndex];
            row.Selected = true;
            var appWrapper = row.DataBoundItem as AppWrapper;
            if (appWrapper == null) return;
            contextApp = appWrapper.App;
            miActivate.Visible = !contextApp.IsActivated;
            miDeactivate.Visible = !contextApp.IsDeactivated;
            miInstall.Visible = !contextApp.IsInstalled;
            miUninstall.Visible = contextApp.IsInstalled;
            miReinstall.Visible = true;
            miDownloadResource.Visible = contextApp.HasResource && !contextApp.IsResourceCached;
            miDeleteResource.Visible = contextApp.HasResource && contextApp.IsResourceCached;
            e.ContextMenuStrip = ctxmAppActions;
        }
    }
}
