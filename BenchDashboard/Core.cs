using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench.Dashboard
{
    public class Core : IDisposable
    {
        public SetupStore SetupStore { get; private set; }

        public IUserInterface UI { get; private set; }

        public BenchConfiguration Configuration { get; private set; }

        public Downloader Downloader { get; private set; }

        public Core(string benchRoot)
        {
            UI = new WinFormsUserInterface();
            SetupStore = new SetupStore();
            Configuration = BenchTasks.PrepareConfiguration(benchRoot, SetupStore, UI);
            Downloader = BenchTasks.InitializeDownloader(Configuration);
        }

        public void DownloadAppResources()
        {
            BenchTasks.DownloadAppResources(Configuration, Downloader);
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            Downloader.Dispose();
        }
    }
}
