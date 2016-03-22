using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public partial class MainForm : Form
    {
        private readonly Core core;
        
        public MainForm(Core core)
        {
            this.core = core;
            InitializeComponent();
            downloadList.Downloader = core.Downloader;
            lblInfo.Text = "Root: " + core.Config.GetStringValue("BenchRoot");
            appLauncherList.Core = core;
            appLauncherList.AppIndex = core.Config.Apps;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            core.DownloadAppResources();
        }
    }
}
