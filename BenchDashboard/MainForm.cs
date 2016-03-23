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
using TsudaKageyu;

namespace Mastersign.Bench.Dashboard
{
    public partial class MainForm : Form
    {
        private readonly Core core;

        public MainForm(Core core)
        {
            this.core = core;
            InitializeComponent();
            InitializeAppLauncherList();
            InitializeTopPanel();
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
                Bitmap cmdImg = null;
                try
                {
                    var extractor = new IconExtractor(core.CmdPath);
                    var icon = extractor.GetIcon(0);
                    cmdImg = new Icon(icon, new Size(16, 16)).ToBitmap();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to load icon for CMD: " + e);
                }
                Bitmap psImg = null;
                try
                {
                    var extractor = new IconExtractor(core.PowerShellPath);
                    var icon = extractor.GetIcon(0);
                    psImg = new Icon(icon, new Size(16, 16)).ToBitmap();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to load icon for PowerShell: " + e);
                }
                BeginInvoke((ThreadStart)(() =>
                {
                    if (cmdImg != null) btnShellCmd.Image = cmdImg;
                    if (psImg != null) btnShellPowerShell.Image = psImg;
                }));
            }).Start();
        }

        private void RootPathClickHandler(object sender, EventArgs e)
        {
            core.ShowPathInExplorer(core.Config.BenchRootDir);
        }

        private void ShellCmdHandler(object sender, EventArgs e)
        {
            core.StartProcess(core.CmdPath);
        }

        private void ShellPowerShellHandler(object sender, EventArgs e)
        {
            core.StartProcess(core.PowerShellPath);
        }

        private void SetupHandler(object sender, EventArgs e)
        {
            new SetupForm(core).ShowDialog(this);
        }
    }
}
