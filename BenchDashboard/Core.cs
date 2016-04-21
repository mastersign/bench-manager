using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public class Core : IDisposable, IBenchManager
    {
        public SetupStore SetupStore { get; private set; }

        public IUserInterface UI { get; private set; }

        public IProcessExecutionHost ProcessExecutionHost { get; private set; }

        public BenchConfiguration Config { get; private set; }

        public BenchEnvironment Env { get; private set; }

        public Downloader Downloader { get; private set; }

        public Control GuiContext { get; set; }

        public event EventHandler ConfigReloaded;

        public event EventHandler AllAppStateChanged;

        public event EventHandler<AppEventArgs> AppStateChanged;

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

        public void SyncWithGui(ThreadStart task)
        {
            if (GuiContext != null && GuiContext.InvokeRequired)
            {
                GuiContext.Invoke(task);
            }
            else
            {
                task();
            }
        }

        private void OnConfigReloaded()
        {
            SyncWithGui(() =>
            {
                var handler = ConfigReloaded;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            });
        }

        private void OnAllAppStateChanged()
        {
            SyncWithGui(() =>
            {
                var handler = AllAppStateChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            });
        }

        private void OnAppStateChanged(string appId)
        {
            SyncWithGui(() =>
            {
                var handler = AppStateChanged;
                if (handler != null)
                {
                    handler(this, new AppEventArgs(appId));
                }
            });
        }

        public void ReloadConfig()
        {
            Config = Config.Reload();
            Env = new BenchEnvironment(Config);
            //Downloader.Dispose();
            //Downloader = BenchTasks.InitializeDownloader(Config);
            OnConfigReloaded();
        }

        public void DownloadAppResources(ProgressCallback progressCb)
        {
            BenchTasks.DownloadAppResources(this, progressCb,
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
                    OnAllAppStateChanged();
                });
        }

        public void DownloadAppResource(ProgressCallback progressCb, string appId)
        {
            BenchTasks.DownloadAppResources(this, progressCb,
                (success, errors) =>
                {
                    if (!success)
                    {
                        UI.ShowWarning("Downloading App Resource",
                            "Downloading app resource failed: "
                            + Environment.NewLine + Environment.NewLine
                            + BuildCombinedErrorMessage(errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId);
        }

        public void DeleteAppResources(ProgressCallback progressCb)
        {
            BenchTasks.DeleteAppResources(this, progressCb,
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
                    OnAllAppStateChanged();
                });
        }

        public void DeleteAppResource(ProgressCallback progressCb, string appId)
        {
            BenchTasks.DeleteAppResources(this, progressCb,
                (success, errors) =>
                {
                    if (!success)
                    {
                        UI.ShowWarning("Deleting App Resource",
                            "Deleting app resource failed: "
                            + Environment.NewLine + Environment.NewLine
                            + BuildCombinedErrorMessage(errors, 10));
                    }
                    OnAppStateChanged(appId);
                }, 
                appId);
        }

        public void InstallApps(ProgressCallback progressCb)
        {
            BenchTasks.RunTasks(this, progressCb,
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
                OnAllAppStateChanged();
            },
            BenchTasks.DownloadAppResources,
            BenchTasks.InstallApps);
        }

        public void InstallApp(ProgressCallback progressCb, string appId)
        {
            BenchTasks.RunTasks(this, progressCb,
            (success, errors) =>
            {
                if (!success)
                {
                    UI.ShowWarning("Installing App",
                        "Installing the app failed: "
                        + Environment.NewLine + Environment.NewLine
                        + BuildCombinedErrorMessage(errors, 10));
                }
                OnAppStateChanged(appId);
            },
            appId,
            BenchTasks.DownloadAppResources,
            BenchTasks.InstallApps);
        }

        public void UninstallApps(ProgressCallback progressCb)
        {
            BenchTasks.UninstallApps(this, progressCb,
            (success, errors) =>
            {
                if (success)
                {
                    UI.ShowInfo("Uninstalling Apps", "Finished.");
                }
                else
                {
                    UI.ShowWarning("Uninstalling Apps",
                        "Uninstalling the following apps failed: "
                        + Environment.NewLine + Environment.NewLine
                        + BuildCombinedErrorMessage(errors, 10));
                }
                OnAllAppStateChanged();
            });
        }

        public void UninstallApp(ProgressCallback progressCb, string appId)
        {
            BenchTasks.UninstallApp(this, progressCb,
            (success, errors) =>
            {
                if (!success)
                {
                    UI.ShowWarning("Uninstalling App",
                        "Uninstalling the app failed: "
                        + Environment.NewLine + Environment.NewLine
                        + BuildCombinedErrorMessage(errors, 10));
                }
                OnAppStateChanged(appId);
            },
            appId);
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
                CommandLine.FormatArgumentList(args),
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
