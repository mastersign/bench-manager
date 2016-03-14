using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Mastersign.Bench.Dashboard
{
    public partial class DownloadList : UserControl
    {
        public DownloadList()
        {
            InitializeComponent();
        }

        private readonly Dictionary<DownloadTask, DownloadControl> downloadControls 
            = new Dictionary<DownloadTask, DownloadControl>();

        private Downloader downloader;

        public Downloader Downloader
        {
            get { return downloader; }
            set
            {
                if (downloader != null)
                {
                    downloadControls.Clear();
                    Controls.Clear();
                }
                downloader = value;
                if (downloader != null)
                {
                    downloader.DownloadStarted += DownloadStartedHandler;
                    downloader.DownloadProgress += DownloadProgressHandler;
                    downloader.DownloadEnded += DownloadEndedHandler;
                }
            }
        }

        private void DownloadStartedHandler(object sender, DownloadEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((EventHandler<DownloadEventArgs>)DownloadStartedHandler, sender, e);
                return;
            }
            var control = new DownloadControl();
            control.FileName = Path.GetFileName(e.Task.TargetFile);
            downloadControls.Add(e.Task, control);
            flowLayout.Controls.Add(control);
        }

        private void DownloadProgressHandler(object sender, DownloadProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((EventHandler<DownloadProgressEventArgs>)DownloadProgressHandler, sender, e);
                return;
            }
            throw new NotImplementedException();
        }

        private void DownloadEndedHandler(object sender, DownloadEndEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke((EventHandler<DownloadEndEventArgs>)DownloadEndedHandler, sender, e);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
