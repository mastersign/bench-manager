using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Mastersign.Bench
{
    public class Downloader : IDisposable
    {
        private readonly int ParallelDownloads;

        private readonly object queueLock = new object();

        private readonly Queue<DownloadTask> queue;

        private readonly WebClient[] activeWebClients;
        private readonly AutoResetEvent[] downloadEvents;
        private volatile int runningDownloads = 0;

        public int ActiveDownloads { get { return runningDownloads; } }

        private readonly Semaphore availableTasks;
        
        public event EventHandler<DownloadEventArgs> DownloadStarted;

        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;

        public event EventHandler<DownloadEndEventArgs> DownloadEnded;

        public WebProxy Proxy { get; set; }

        public Downloader()
            : this(1)
        {
        }

        public Downloader(int parallelDownloads)
        {
            if (parallelDownloads < 1 || parallelDownloads > 9999)
            {
                throw new ArgumentOutOfRangeException("parallelDownloads",
                    "The number of parallel downloads must be at least 1 and less than 10000.");
            }
            ParallelDownloads = parallelDownloads;
            activeWebClients = new WebClient[parallelDownloads];
            downloadEvents = new AutoResetEvent[parallelDownloads];
            for (int i = 0; i < parallelDownloads; i++)
            {
                downloadEvents[i] = new AutoResetEvent(false);
            }
            availableTasks = new Semaphore(0, int.MaxValue);

            for (int i = 0; i < parallelDownloads; i++)
            {
                new Thread(() => Worker(i)).Start();
            }
        }

        private void OnDownloadStarted(DownloadTask task)
        {
            var handler = DownloadStarted;
            if (handler != null)
            {
                handler(this, new DownloadEventArgs(task));
            }
        }

        private void OnDownloadProgress(DownloadTask task, long bytesDownloaded, long totalBytes, int percentage)
        {
            var handler = DownloadProgress;
            if (handler != null)
            {
                handler(this, new DownloadProgressEventArgs(task, bytesDownloaded, totalBytes, percentage));
            }
        }

        private void OnDownloadEnded(DownloadTask task, string errorMessage = null)
        {
            var handler = DownloadEnded;
            if (handler != null)
            {
                handler(this, new DownloadEndEventArgs(task, errorMessage));
            }
        }

        public void Enqueue(DownloadTask task)
        {
            lock (queueLock)
            {
                if (IsDisposed) { throw new ObjectDisposedException(GetType().Name); }
                queue.Enqueue(task);
            }
            availableTasks.Release();
        }

        private void Worker(int no)
        {
            DownloadTask task = null;
            var wc = new WebClient { Proxy = Proxy };
            wc.DownloadProgressChanged += (o, e) =>
            {
                task.DownloadedBytes = e.BytesReceived;
                OnDownloadProgress(task, e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
            };
            wc.DownloadFileCompleted += (o, e) =>
            {
                if (e.Cancelled)
                {
                    OnDownloadEnded(task, "Cancelled");
                }
                else if (e.Error != null)
                {
                    OnDownloadEnded(task, e.Error.Message);
                }
                else
                {
                    OnDownloadEnded(task);
                }
                runningDownloads--;
                downloadEvents[no].Set();
            };

            while (true)
            {
                availableTasks.WaitOne();

                // dispose synchronization resources if canceled
                if (IsDisposed)
                {
                    downloadEvents[no].Close();
                    downloadEvents[no] = null;
                    break;
                }

                // Aquire next available task
                lock (queueLock)
                {
                    task = queue.Dequeue();
                }
                runningDownloads++;

                // Start download
                OnDownloadStarted(task);
                wc.DownloadFileAsync(task.Url, task.TargetFile);

                // Wait for end
                downloadEvents[no].WaitOne();
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            queue.Clear();

            // allow worker to end
            availableTasks.Release(ParallelDownloads);
        }
    }

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

    public class DownloadTask
    {
        public string Id { get; private set; }

        public Uri Url { get; private set; }

        public string TargetFile { get; private set; }

        public long DownloadedBytes { get; set; }

        public bool HasEnded { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public DownloadTask(string id, Uri url, string targetFile)
        {
            Id = id;
            Url = url;
            TargetFile = targetFile;
        }
    }
}
