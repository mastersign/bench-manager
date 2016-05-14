using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Mastersign.Bench.Dashboard.Properties;
using TsudaKageyu;

namespace Mastersign.Bench.Dashboard
{
    public partial class MainForm : Form
    {
        private readonly Core core;

        private SetupForm setupForm;

        public MainForm(Core core)
        {
            this.core = core;
            core.AllAppStateChanged += AppStateChangedHandler;
            core.AppStateChanged += AppStateChangedHandler;
            core.ConfigReloaded += AppStateChangedHandler;
            InitializeComponent();
            InitializeAppLauncherList();
            InitializeTopPanel();
            InitializeStatusStrip();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (core.SetupOnStartup)
            {
                SetupHandler(this, EventArgs.Empty);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (core.Busy)
            {
                core.Cancelation.Cancel();
                e.Cancel = true;
                MessageBox.Show(this,
                    "You can not close this window until the current running setup action has ended. The action has been requested to cancel.",
                    "Closing Setup Window",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AppStateChangedHandler(object sender, EventArgs e)
        {
            InitializeAppLauncherList();
            InitializeStatusStrip();
        }

        private void InitializeAppLauncherList()
        {
            appLauncherList.Core = core;
            appLauncherList.AppIndex = core.Config.Apps;
        }

        private void InitializeStatusStrip()
        {
            tsslRootPath.Text = core.Config.BenchRootDir;
            tsslAppCount.Text = core.Config.Apps.ActiveApps.Length.ToString();
        }

        private void InitializeTopPanel()
        {
            new Thread(() =>
            {
                var cmdImg = ExtractIcon(core.CmdPath, "CMD");
                var psImg = ExtractIcon(core.PowerShellPath, "PowerShell");
                BeginInvoke((ThreadStart)(() =>
                {
                    btnShellCmd.Image = cmdImg ?? Resources.missing_app_16;
                    btnShellPowerShell.Image = psImg ?? Resources.missing_app_16;
                }));
            }).Start();
        }

        private static Bitmap ExtractIcon(string path, string name)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var extractor = new IconExtractor(path);
                var icon = extractor.GetIcon(0);
                return new Icon(icon, new Size(16, 16)).ToBitmap();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to load icon for " + name + ": " + e);
                return null;
            }
        }

        private void RootPathClickHandler(object sender, EventArgs e)
        {
            core.ShowPathInExplorer(core.Config.BenchRootDir);
        }

        private void ShellCmdHandler(object sender, EventArgs e)
        {
            new DefaultExecutionHost().StartProcess(core.Env,
                core.Config.GetStringValue(PropertyKeys.ProjectRootDir),
                core.CmdPath, "", result => { }, ProcessMonitoring.ExitCode);
        }

        private void ShellPowerShellHandler(object sender, EventArgs e)
        {
            new DefaultExecutionHost().StartProcess(core.Env,
                core.Config.GetStringValue(PropertyKeys.ProjectRootDir),
                core.PowerShellPath, "", result => { }, ProcessMonitoring.ExitCode);
        }

        private void ShellBashHandler(object sender, EventArgs e)
        {
            var bashPath = core.BashPath;
            if (File.Exists(bashPath))
            {
                new DefaultExecutionHost().StartProcess(core.Env,
                    core.Config.GetStringValue(PropertyKeys.ProjectRootDir),
                    bashPath, "", result => { }, ProcessMonitoring.ExitCode);
            }
            else
            {
                MessageBox.Show(
                    "The executable of Bash was not found in the Git distribution."
                    + Environment.NewLine + Environment.NewLine
                    + bashPath,
                    "Running Bash...",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetupHandler(object sender, EventArgs e)
        {
            if (setupForm == null || setupForm.IsDisposed)
            {
                setupForm = new SetupForm(core);
            }
            if (!setupForm.Visible) setupForm.Show();
        }

        private void AboutHandler(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }
    }
}
