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

        private readonly Dictionary<string, AppWrapper> appLookup = new Dictionary<string, AppWrapper>();

        public SetupForm(Core core)
        {
            this.core = core;
            core.ConfigReloaded += ConfigReloadedHandler;
            core.AllAppStateChanged += ConfigReloadedHandler;
            core.AppStateChanged += AppStateChangedHandler;
            core.BusyChanged += CoreBusyChangedHandler;
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

        private void AppStateChangedHandler(object sender, AppEventArgs e)
        {
            AppWrapper wrapper;
            if (appLookup.TryGetValue(e.ID, out wrapper))
            {
                wrapper.App.DiscardCachedValues();
                wrapper.NotifyChanges();
            }
        }

        private void CoreBusyChangedHandler(object sender, EventArgs e)
        {
            var notBusy = !core.Busy;
            foreach (ToolStripItem tsmi in tsmSetup.DropDownItems)
            {
                tsmi.Enabled = notBusy;
            }
            foreach (ToolStripItem mi in ctxmAppActions.Items)
            {
                mi.Enabled = notBusy;
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
            appLookup.Clear();
            var list = new List<AppWrapper>();
            var cnt = 0;
            foreach (var app in core.Config.Apps)
            {
                cnt++;
                var wrapper = new AppWrapper(app, cnt);
                list.Add(wrapper);
                appLookup[app.ID] = wrapper;
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
            core.ReinstallApps(ProgressInfoHandler);
        }

        private void UpgradeAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade apps");
            core.UpgradeApps(ProgressInfoHandler);
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
            core.ReinstallApp(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void UpgradeAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade app " + contextApp.ID);
            core.UpgradeApp(ProgressInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void UpgradePackageHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade app " + contextApp.ID);
            core.UpgradeApp(ProgressInfoHandler, contextApp.ID);
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

        private void RefreshViewHandler(object sender, EventArgs e)
        {
            core.ReloadConfig();
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

            miInstall.Visible =  contextApp.CanCheckInstallation && !contextApp.IsInstalled;
            miReinstall.Visible = contextApp.Typ == AppTyps.Default
                && contextApp.CanCheckInstallation && contextApp.IsInstalled
                && contextApp.HasResource;
            miUpgrade.Visible = contextApp.Typ == AppTyps.Default
                && contextApp.CanCheckInstallation && contextApp.IsInstalled
                && contextApp.HasResource && !contextApp.IsVersioned;
            miPackageUpgrade.Visible = contextApp.Typ != AppTyps.Default
                && contextApp.CanCheckInstallation && contextApp.IsInstalled;
            miUninstall.Visible = contextApp.CanCheckInstallation && contextApp.IsInstalled;

            miDownloadResource.Visible = contextApp.HasResource && !contextApp.IsResourceCached;
            miDeleteResource.Visible = contextApp.HasResource && contextApp.IsResourceCached;
            e.ContextMenuStrip = ctxmAppActions;
        }
    }
}
