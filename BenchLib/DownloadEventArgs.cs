using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class DownloadEventArgs : EventArgs
    {
        public DownloadTask Task { get; private set; }

        public DownloadEventArgs(DownloadTask task)
        {
            Task = task;
        }
    }

    public class DownloadProgressEventArgs : DownloadEventArgs
    {
        public long LoadedBytes { get; private set; }

        public long TotalBytes { get; private set; }

        public int Percentage { get; private set; }

        public DownloadProgressEventArgs(DownloadTask task, long loadedBytes, long totalBytes, int percentage)
            : base(task)
        {
            LoadedBytes = loadedBytes;
            TotalBytes = totalBytes;
            Percentage = percentage;
        }
    }

    public class DownloadEndEventArgs : DownloadEventArgs
    {
        public string ErrorMessage { get; private set; }

        public bool HasFailed { get { return ErrorMessage != null; } }

        public DownloadEndEventArgs(DownloadTask task, string errorMessage = null)
            : base(task)
        {
            ErrorMessage = errorMessage;
        }
    }
}
