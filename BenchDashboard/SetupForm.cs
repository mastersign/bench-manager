extern alias v40async;
using v40async::ConEmu.WinForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
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

        private ConEmuExecutionHost conHost;
        private ConEmuControl conControl;

        public SetupForm(Core core)
        {
            this.core = core;
            core.ConfigReloaded += ConfigReloadedHandler;
            core.AllAppStateChanged += ConfigReloadedHandler;
            core.AppStateChanged += AppStateChangedHandler;
            core.BusyChanged += CoreBusyChangedHandler;
            InitializeComponent();
            InitializeConsole();
            gridApps.AutoGenerateColumns = false;
        }

        private void SetupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisposeConsole();
            core.ConfigReloaded -= ConfigReloadedHandler;
            core.AllAppStateChanged -= AllAppStateChangedHandler;
            core.AppStateChanged -= AppStateChangedHandler;
            core.BusyChanged -= CoreBusyChangedHandler;
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            InitializeDownloadList();
            InitializeBounds();
            InitializeAppList();
            UpdatePendingCounts();
        }

        private void InitializeBounds()
        {
            var region = Screen.PrimaryScreen.WorkingArea;
            var w = Math.Max(MinimumSize.Width, region.Width / 2);
            var h = Math.Max(MinimumSize.Height, region.Height);
            var x = region.Right - w;
            var y = region.Top;
            SetBounds(x, y, w, h);
        }

        private void ConfigReloadedHandler(object sender, EventArgs e)
        {
            InitializeAppList();
            UpdatePendingCounts();
        }

        private void AllAppStateChangedHandler(object sender, EventArgs e)
        {
            foreach (var app in core.Config.Apps)
            {
                NotifyAppStateChange(app.ID);
            }
            UpdatePendingCounts();
        }

        private void AppStateChangedHandler(object sender, AppEventArgs e)
        {
            NotifyAppStateChange(e.ID);
            UpdatePendingCounts();
        }

        private void NotifyAppStateChange(string appId)
        {
            AppWrapper wrapper;
            if (appLookup.TryGetValue(appId, out wrapper))
            {
                wrapper.App.DiscardCachedValues();
                wrapper.NotifyChanges();
            }
        }

        private void UpdatePendingCounts()
        {
            var pendingDownloads = 0;
            var pendingInstalls = 0;
            var pendingUninstalls = 0;
            foreach (var app in core.Config.Apps)
            {
                if (app.IsActive && app.CanDownloadResource) pendingDownloads++;
                if (app.IsActive && app.CanInstall) pendingInstalls++;
                if (!app.IsActive && app.CanUninstall) pendingUninstalls++;
            }
            var list = new List<string>();
            if (pendingUninstalls > 0)
            {
                list.Add(string.Format("{0} {1}",
                    pendingUninstalls,
                    pendingUninstalls == 1 ? "Uninstall" : "Uninstalls"));
            }
            if (pendingDownloads > 0)
            {
                list.Add(string.Format("{0} {1}",
                    pendingDownloads,
                    pendingDownloads == 1 ? "Download" : "Downloads"));
            }
            if (pendingInstalls > 0)
            {
                list.Add(string.Format("{0} {1}",
                    pendingInstalls,
                    pendingInstalls == 1 ? "Install" : "Installs"));
            }
            lblPending.Text = list.Count > 0 ? string.Join(", ", list) : "Nothing";
        }

        private void CoreBusyChangedHandler(object sender, EventArgs e)
        {
            var notBusy = !core.Busy;
            foreach (ToolStripItem tsmi in tsmSetup.DropDownItems)
            {
                tsmi.Enabled = notBusy;
            }
            foreach (ToolStripItem tsmi in tsmEdit.DropDownItems)
            {
                tsmi.Enabled = notBusy;
            }
            foreach (ToolStripItem mi in ctxmAppActions.Items)
            {
                mi.Enabled = notBusy;
            }
            btnAuto.Enabled = notBusy;
        }

        private void InitializeConsole()
        {
            var c = new ConEmuControl();
            c.Dock = DockStyle.Bottom;
            c.Height = 200;
            c.AutoStartInfo = null;
            c.Visible = true;
            Controls.Add(c);
            conControl = c;
            conHost = new ConEmuExecutionHost(conControl, core.Config.Apps[AppKeys.ConEmu].Exe);
            core.ProcessExecutionHost = conHost;
        }

        private void DisposeConsole()
        {
            core.ProcessExecutionHost = new DefaultExecutionHost();
        }

        private void InitializeDownloadList()
        {
            downloadList.Downloader = core.Downloader;
            core.Downloader.IsWorkingChanged += DownloaderIsWorkingChangedHandler;
            IsDownloadListVisible = false;
        }

        private void InitializeAppList()
        {
            AsyncManager.StartTask(() =>
            {
                appLookup.Clear();
                var list = new List<AppWrapper>();
                var cnt = 0;
                foreach (var app in core.Config.Apps)
                {
                    cnt++;
                    app.LoadCachedValues();
                    var wrapper = new AppWrapper(app, cnt);
                    list.Add(wrapper);
                    appLookup[app.ID] = wrapper;
                }

                var bindingList = new SortedBindingList<AppWrapper>(list);

                BeginInvoke((ThreadStart)(() =>
                {
                    var selectedRow = gridApps.SelectedRows.Count > 0 ? gridApps.SelectedRows[0].Index : -10;
                    gridApps.DataSource = bindingList;
                    if (sortedColumn != null)
                    {
                        gridApps.Sort(sortedColumn, sortDirection);
                    }
                    if (selectedRow >= 0 && gridApps.Rows.Count >= selectedRow + 1)
                    {
                        gridApps.Rows[selectedRow].Selected = true;
                    }
                }));
            });
        }

        private void UpdateProgressBar(float progress)
        {
            if (float.IsNaN(progress) || float.IsInfinity(progress)) progress = 0f;
            progress = Math.Min(1f, Math.Max(0f, progress));
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
            if (core.Busy) throw new InvalidOperationException("The core is already busy.");
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
            core.Busy = true;
            AsyncManager.StartTask(() =>
            {
                core.UI.EditTextFile(path);
                core.Busy = false;
                core.Reload(path == core.Config.GetStringValue(PropertyKeys.CustomConfigFile));
            });
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

        private void TaskInfoHandler(TaskInfo info)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<TaskInfo>)TaskInfoHandler, info);
                return;
            }
            lblInfo.Text = info.Message;
            var progressInfo = info as TaskProgress;
            if (progressInfo != null)
            {
                UpdateProgressBar(progressInfo.Progress);
            }
        }

        private void AnnounceTask(string label)
        {
            lblTask.Text = label;
        }

        private async void AutoHandler(object sender, EventArgs e)
        {
            AnnounceTask("Automatic Setup");
            await core.AutoSetupAsync(TaskInfoHandler);
        }

        private async void DownloadAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Download App Resources");
            await core.DownloadAppResourcesAsync(TaskInfoHandler);
        }

        private async void DeleteAllResourcesHandler(object sender, EventArgs e)
        {
            AnnounceTask("Delete App Resources");
            await core.DeleteAppResourcesAsync(TaskInfoHandler);
        }

        private async void InstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Install Apps");
            await core.InstallAppsAsync(TaskInfoHandler);
        }

        private async void UninstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Uninstall Apps");
            await core.UninstallAppsAsync(TaskInfoHandler);
        }

        private async void ReinstallAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Reinstall Apps");
            await core.ReinstallAppsAsync(TaskInfoHandler);
        }

        private async void UpgradeAllHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade Apps");
            await core.UpgradeAppsAsync(TaskInfoHandler);
        }

        private async void UpdateEnvironment(object sender, EventArgs e)
        {
            AnnounceTask("Update Environment");
            await core.UpdateEnvironmentAsync(TaskInfoHandler);
        }

        private async void InstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Install App " + contextApp.ID);
            await core.InstallAppsAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private async void ReinstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Reinstall App " + contextApp.ID);
            await core.ReinstallAppsAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private async void UpgradeAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade App " + contextApp.ID);
            await core.UpgradeAppsAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private async void UpgradePackageHandler(object sender, EventArgs e)
        {
            AnnounceTask("Upgrade App " + contextApp.ID);
            await core.UpgradeAppsAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private async void DownloadAppResourceHandler(object sender, EventArgs e)
        {
            AnnounceTask("Download App Resource for " + contextApp.ID);
            await core.DownloadAppResourcesAsync(contextApp.ID, TaskInfoHandler);
            contextApp = null;
        }

        private async void DeleteAppResourceHandler(object sender, EventArgs e)
        {
            AnnounceTask("Delete App Resource for " + contextApp.ID);
            await core.DeleteAppResourcesAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private async void UninstallAppHandler(object sender, EventArgs e)
        {
            AnnounceTask("Uninstall App " + contextApp.ID);
            await core.UninstallAppsAsync(TaskInfoHandler, contextApp.ID);
            contextApp = null;
        }

        private void RefreshViewHandler(object sender, EventArgs e)
        {
            core.Reload();
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
            if (e.RowIndex < 0) return;
            var row = gridApps.Rows[e.RowIndex];
            row.Selected = true;
            var appWrapper = row.DataBoundItem as AppWrapper;
            if (appWrapper == null) return;
            contextApp = appWrapper.App;

            miInstall.Visible = contextApp.CanInstall;
            miReinstall.Visible = contextApp.CanReinstall;
            miUpgrade.Visible = contextApp.CanUpgrade;
            miPackageUpgrade.Visible = contextApp.IsInstalled && contextApp.IsManagedPackage;
            miUninstall.Visible = contextApp.CanUninstall;

            miDownloadResource.Visible = contextApp.CanDownloadResource;
            miDeleteResource.Visible = contextApp.CanDeleteResource;

            tsSeparatorDownloads.Visible =
                (miInstall.Visible
                    || miReinstall.Visible
                    || miUpgrade.Visible
                    || miPackageUpgrade.Visible
                    || miUninstall.Visible)
                && (miDownloadResource.Visible
                    || miDeleteResource.Visible);

            e.ContextMenuStrip = ctxmAppActions;
        }

        private void gridApps_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (core.Busy) return;
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            var col = gridApps.Columns[e.ColumnIndex];
            if (col == colActivated || col == colExcluded)
            {
                var row = gridApps.Rows[e.RowIndex];
                var appWrapper = row.DataBoundItem as AppWrapper;
                if (col == colActivated)
                {
                    core.SetAppActivated(appWrapper.ID, !appWrapper.App.IsActivated);
                }
                if (col == colExcluded)
                {
                    core.SetAppDeactivated(appWrapper.ID, !appWrapper.App.IsDeactivated);
                }
            }
        }
    }
}
