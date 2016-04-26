using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public class Core : IDisposable, IBenchManager
    {
        public SetupStore SetupStore { get; private set; }

        public IUserInterface UI { get; private set; }

        public IProcessExecutionHost ProcessExecutionHost { get; set; }

        public BenchConfiguration Config { get; private set; }

        public BenchEnvironment Env { get; private set; }

        public Downloader Downloader { get; private set; }

        public Control GuiContext { get; set; }

        private bool busy;

        public event EventHandler ConfigReloaded;

        public event EventHandler AllAppStateChanged;

        public event EventHandler<AppEventArgs> AppStateChanged;

        public event EventHandler BusyChanged;

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

        public bool Busy
        {
            get { return busy; }
            set
            {
                if (value == busy) return;
                busy = value;
                OnBusyChanged();
            }
        }

        private void OnBusyChanged()
        {
            SyncWithGui(() =>
            {
                var handler = BusyChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            });
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

        public void Reload(bool configChanged = false)
        {
            Config = Config.Reload();
            Env = new BenchEnvironment(Config);
            if (configChanged)
            {
                Downloader.Dispose();
                Downloader = BenchTasks.InitializeDownloader(Config);
            }
            OnConfigReloaded();
        }

        public void SetAppActivated(string appId, bool value)
        {
            var activationFile = new ActivationFile(Config.GetStringValue(PropertyKeys.AppActivationFile));
            if (value)
            {
                activationFile.SignIn(appId);
            }
            else
            {
                activationFile.SignOut(appId);
            }
            Reload();
        }

        public void SetAppDeactivated(string appId, bool value)
        {
            var deactivationFile = new ActivationFile(Config.GetStringValue(PropertyKeys.AppDeactivationFile));
            if (value)
            {
                deactivationFile.SignIn(appId);
            }
            else
            {
                deactivationFile.SignOut(appId);
            }
            Reload();
        }

        public Task<TaskResult> DoTaskAsync(string taskLabel, BenchTask action,
            IBenchManager man, ICollection<AppFacade> apps,
            Action<TaskInfo> notify, Cancelation cancelation)
        {
            var t = new Task<TaskResult>(() =>
            {
                var infos = new List<TaskInfo>();
                action(man, apps,
                    info =>
                    {
                        infos.Add(info);
                        if (notify != null)
                        {
                            notify(info);
                        }
                    },
                    cancelation);
                return new TaskResult(taskLabel, infos, cancelation.IsCanceled);
            });
            t.Start();
            return t;
        }

        public void AutoSetup(Action<TaskInfo> notify, Cancelation cancelation)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;

            var activeApps = Config.Apps.ActiveApps;
            var inactiveApps = Config.Apps.InactiveApps;

            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Installing Apps", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Installing Apps",
                            BuildCombinedErrorMessage(
                                "Installing the following apps failed:",
                                "Installing the apps failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                },
                new ICollection<AppFacade>[] { inactiveApps, activeApps },
                BenchTasks.UninstallApps,
                BenchTasks.DownloadAppResources,
                BenchTasks.InstallApps,
                BenchTasks.UpdateEnvironment);
        }

        public void DownloadAppResources(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.DownloadAppResources(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Downloading App Resources", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Downloading App Resources",
                            BuildCombinedErrorMessage(
                                "Downloading resources for the following apps failed:",
                                "Downloading the app resources failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                });
        }

        public void DownloadAppResource(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.DownloadAppResources(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Downloading App Resource",
                            BuildCombinedErrorMessage(
                                "Downloading app resource failed:",
                                "Downloading the resource for app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId);
        }

        public void DeleteAppResources(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.DeleteAppResources(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Deleting App Resources", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Deleting App Resources",
                            BuildCombinedErrorMessage(
                                "Deleting resources for the following apps failed:",
                                "Deleting the app resources failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                });
        }

        public void DeleteAppResource(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.DeleteAppResources(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Deleting App Resource",
                            BuildCombinedErrorMessage(
                                "Deleting app resource failed:",
                                "Deleting the resource of app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId);
        }

        public void InstallApps(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Installing Apps", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Installing Apps",
                            BuildCombinedErrorMessage(
                                "Installing the following apps failed:",
                                "Installing the apps failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                },
                BenchTasks.DownloadAppResources,
                BenchTasks.InstallApps);
        }

        public void InstallApp(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Installing App",
                            BuildCombinedErrorMessage(
                                "Installing the app failed:",
                                "Installing the app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId,
                BenchTasks.DownloadAppResources,
                BenchTasks.InstallApps);
        }

        public void ReinstallApps(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Reinstalling Apps", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Reinstalling Apps",
                            BuildCombinedErrorMessage(
                                "Reinstalling the following apps failed:",
                                "Reinstalling the apps failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                },
                BenchTasks.DownloadAppResources,
                BenchTasks.UninstallApps,
                BenchTasks.InstallApps);
        }

        public void ReinstallApp(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Reinstall App",
                            BuildCombinedErrorMessage(
                                "Reinstalling the app failed:",
                                "Reinstalling the app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId,
                BenchTasks.DownloadAppResources,
                BenchTasks.UninstallApps,
                BenchTasks.InstallApps);
        }

        public void UpgradeApps(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Upgrading Apps", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Upgrading Apps",
                            BuildCombinedErrorMessage(
                                "Upgrading the following apps failed:",
                                "Upgrading the apps failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                },
                BenchTasks.DeleteAppResources,
                BenchTasks.DownloadAppResources,
                BenchTasks.UninstallApps,
                BenchTasks.InstallApps);
        }

        public void UpgradeApp(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Upgrade App",
                            BuildCombinedErrorMessage(
                                "Upgrading the app failed:",
                                "Upgrading the app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId,
                BenchTasks.DeleteAppResources,
                BenchTasks.DownloadAppResources,
                BenchTasks.UninstallApps,
                BenchTasks.InstallApps);
        }

        public void UninstallApps(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Uninstalling Apps", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Uninstalling Apps",
                            BuildCombinedErrorMessage(
                                "Uninstalling the following apps failed:",
                                "Uninstalling the apps failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                },
                BenchTasks.UninstallApps);
        }

        public void UninstallApp(ProgressCallback progressCb, string appId)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.RunTasks(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (!success)
                    {
                        UI.ShowWarning("Uninstalling App",
                            BuildCombinedErrorMessage(
                                "Uninstalling the app failed:",
                                "Uninstalling the app " + appId + " failed.",
                                errors, 10));
                    }
                    OnAppStateChanged(appId);
                },
                appId,
                BenchTasks.UninstallApps);
        }

        public void UpdateEnvironment(ProgressCallback progressCb)
        {
            if (Busy) throw new InvalidOperationException("The core is already busy.");
            Busy = true;
            BenchTasks.UpdateEnvironment(this, progressCb,
                (success, errors) =>
                {
                    Busy = false;
                    if (success)
                    {
                        UI.ShowInfo("Updating Environment", "Finished.");
                    }
                    else
                    {
                        UI.ShowWarning("Updating Environment",
                            BuildCombinedErrorMessage(
                                "Updating the bench environment for the following apps failed:",
                                "Updating the bench environment failed.",
                                errors, 10));
                    }
                    OnAllAppStateChanged();
                });
        }

        private static string BuildCombinedErrorMessage(string infoWithErrors, string infoWithoutErrors,
            IEnumerable<AppTaskError> errors, int maxLines)
        {
            var sb = new StringBuilder();
            var cnt = 0;
            if (errors != null)
            {
                foreach (var err in errors)
                {
                    cnt++;
                    if (cnt >= maxLines)
                    {
                        sb.AppendLine("...");
                        break;
                    }
                    sb.AppendLine(err.ToString());
                }
            }
            return cnt > 0
                ? infoWithErrors + Environment.NewLine + Environment.NewLine + sb.ToString()
                : infoWithoutErrors;
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
                result => { },
                ProcessMonitoring.ExitCode);
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
