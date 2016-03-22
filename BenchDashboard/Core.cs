using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench.Dashboard
{
    public class Core : IDisposable
    {
        public SetupStore SetupStore { get; private set; }

        public IUserInterface UI { get; private set; }

        public BenchConfiguration Config { get; private set; }

        public Downloader Downloader { get; private set; }

        public Core(string benchRoot)
        {
            UI = new WinFormsUserInterface();
            SetupStore = new SetupStore();
            Config = BenchTasks.PrepareConfiguration(benchRoot, SetupStore, UI);
            Downloader = BenchTasks.InitializeDownloader(Config);
        }

        public void DownloadAppResources()
        {
            BenchTasks.DownloadAppResources(Config, Downloader);
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
