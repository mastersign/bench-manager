﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
            Debug.WriteLine("Initializing UI Core for Bench...");
            UI = new WinFormsUserInterface();
            SetupStore = new SetupStore();
            Config = BenchTasks.PrepareConfiguration(benchRoot, SetupStore, UI);
            Env = new BenchEnvironment(Config);
            Downloader = BenchTasks.InitializeDownloader(Config);
        }

        public void DownloadAppResources()
        {
            BenchTasks.DownloadAppResources(Config, Downloader,
                (success, errors) =>
                {
                    if (success)
                    {
                        UI.ShowInfo("Downloading App Resources", "Finished.");
                    }
                    else
                    {
                        var errorLines = new List<string>();
                        foreach (var err in errors)
                        {
                            if (errorLines.Count == 10)
                            {
                                errorLines.Add("...");
                                break;
                            }
                            errorLines.Add(err.ToString());
                        }
                        UI.ShowWarning("Download App Resources",
                            "Downloading the resources for the following apps failed: "
                            + Environment.NewLine + Environment.NewLine
                            + string.Join(Environment.NewLine, errorLines.ToArray()));
                    }
                });
        }

        public Process LaunchApp(string id, params string[] args)
        {
            try
            {
                return BenchTasks.LaunchApp(Config, Env, id, args);
            }
            catch (FileNotFoundException e)
            {
                UI.ShowWarning("Launching App",
                    "The executable of the app could not be found."
                     + Environment.NewLine + Environment.NewLine
                     + e.FileName);
                return null;
            }
        }

        public Process StartProcess(string exe, params string[] args)
        {
            return BenchTasks.StartProcess(Env, Config.BenchRootDir, exe, args);
        }

        public void ShowPathInExplorer(string path)
        {
            Process.Start(path);
        }

        public string CmdPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetEnvironmentVariable("SystemRoot"),
                    @"System32\cmd.exe");
            }
        }

        public string PowerShellPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetEnvironmentVariable("SystemRoot"),
                    @"System32\WindowsPowerShell\v1.0\powershell.exe");
            }
        }

        public string BashPath
        {
            get
            {
                return Path.Combine(
                    Config.GetStringGroupValue(AppKeys.Git, PropertyKeys.AppDir),
                    @"bin\bash.exe");
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
            UI.ShowError("Unexpected Exception",
                message
                + Environment.NewLine + Environment.NewLine
                + e.ToString());
        }
    }
}
