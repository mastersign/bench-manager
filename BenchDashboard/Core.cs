using System;
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

        public IProcessExecutionHost ProcessExecutionHost { get; private set; }

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
            ProcessExecutionHost = new DefaultExecutionHost();
        }

        public void DownloadAppResources(ProgressCallback progressCb)
        {
            BenchTasks.DownloadAppResources(Config, Downloader, progressCb,
                (success, errors) =>
                {
                    if (success)
                    {
                        UI.ShowInfo("Downloading App Resources", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Downloading App Resources",
                            "Downloading resources for the following apps failed: "
                            + Environment.NewLine + Environment.NewLine
                            + BuildCombinedErrorMessage(errors, 10));
                    }
                });
        }

        public void DeleteAppResources(ProgressCallback progressCb)
        {
            BenchTasks.DeleteAppResources(Config, progressCb,
                (success, errors) =>
                {
                    if (success)
                    {
                        UI.ShowInfo("Deleting App Resources", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Deleting App Resources",
                            "Deleting resources for the following apps failed: "
                            + Environment.NewLine + Environment.NewLine
                            + BuildCombinedErrorMessage(errors, 10));
                    }
                });
        }

        public void InstallApps(ProgressCallback progressCb)
        {
            BenchTasks.InstallApps(Config, ProcessExecutionHost, progressCb,
            (success, errors) =>
            {
                if (success)
                {
                    UI.ShowInfo("Installing Apps", "Finished.");
                }
                else
                {
                    UI.ShowWarning("Installing Apps",
                        "Installing the following apps failed: "
                        + Environment.NewLine + Environment.NewLine
                        + BuildCombinedErrorMessage(errors, 10));
                }
            });
        }

        private static string BuildCombinedErrorMessage(IEnumerable<AppTaskError> errors, int maxLines)
        {
            var errorLines = new List<string>(maxLines);
            foreach (var err in errors)
            {
                if (errorLines.Count >= maxLines - 1)
                {
                    errorLines.Add("...");
                    break;
                }
                errorLines.Add(err.ToString());
            }
            return string.Join(Environment.NewLine, errorLines.ToArray());
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

        public void StartProcess(string exe, params string[] args)
        {
            ProcessExecutionHost.StartProcess(Env, Config.BenchRootDir, exe,
                BenchTasks.FormatCommandLineArguments(args),
                exitCode => { });
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
