﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public class Core : IDisposable
    {
        public SetupStore SetupStore { get; private set; }

        public IUserInterface UI { get; private set; }

        public BenchConfiguration Config { get; private set; }

        public BenchEnvironment Env { get; private set; }

        public Downloader Downloader { get; private set; }

        public Core(string benchRoot)
        {
            UI = new WinFormsUserInterface();
            SetupStore = new SetupStore();
            Config = BenchTasks.PrepareConfiguration(benchRoot, SetupStore, UI);
            Env = new BenchEnvironment(Config);
            Downloader = BenchTasks.InitializeDownloader(Config);
        }

        public void DownloadAppResources()
        {
            BenchTasks.DownloadAppResources(Config, Downloader);
        }

        public Process LaunchApp(string id, params string[] args)
        {
            try
            {
                return BenchTasks.LaunchApp(Config, Env, id, args);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("The executable of the app could not be found."
                     + Environment.NewLine + Environment.NewLine
                     + e.FileName,
                     "Launching App Failed",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            Downloader.Dispose();
        }

        [Conditional("DEBUG")]
        public void DisplayError(string message, Exception e)
        {
            MessageBox.Show(message 
                + Environment.NewLine + Environment.NewLine 
                + e.ToString(),
                "Catched unexpected exception...",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
