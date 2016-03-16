using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public partial class MainForm : Form
    {
        private BenchConfiguration config;
        private Downloader downloader;

        public MainForm()
        {
            InitializeComponent();
        }

        private BenchConfiguration Configuration
        {
            get
            {
                if (config == null)
                {
                    config = new BenchConfiguration(@"F:\bench-dev");
                }
                return config;
            }
        }

        private Downloader Downloader
        {
            get
            {
                if (downloader == null)
                {
                    var parallelDownloads = Configuration.GetInt32Value(PropertyKeys.ParallelDownloads, 1);
                    var downloadAttempts = Configuration.GetInt32Value(PropertyKeys.DownloadAttempts, 1);
                    downloader = new Downloader(parallelDownloads);
                    downloader.DownloadAttempts = downloadAttempts;
                    downloadList.Downloader = downloader;
                }
                return downloader;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            BenchTasks.DownloadAppResources(Configuration, Downloader);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloader != null)
            {
                downloader.Dispose();
            }
        }
    }
}
